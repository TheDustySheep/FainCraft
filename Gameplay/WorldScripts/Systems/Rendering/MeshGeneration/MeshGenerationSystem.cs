using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Clusters;
using FainCraft.Gameplay.WorldScripts.Signals;
using FainCraft.Gameplay.WorldScripts.Storage;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;
using FainEngine_v2.Collections;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
internal class MeshGenerationSystem : IMeshGenerationSystem, IDisposable
{
    const int ACTIVE_COUNT = 128;

    private readonly CancellationTokenSource cts = new CancellationTokenSource();

    private readonly HashSet<ChunkCoord> _activeChunks = new();
    private readonly HashSet<ChunkCoord> _queue = new();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(ACTIVE_COUNT);
    private readonly LimitedObjectPool<GenerationData> _dataPool = new(ACTIVE_COUNT);

    private readonly IActiveRegionRadius _activeRegions;
    private readonly IRenderSystem _renderSystem;
    private readonly IMeshGenerator _generator;
    private readonly IChunkClusterDataStore _clusterDataStore;
    private readonly IEventBus _signalBus;

    public MeshGenerationSystem(IServiceProvider provider, IActiveRegionRadius activeRegions)
    {
        _activeRegions    = activeRegions;
        _renderSystem     = provider.Get<IRenderSystem>();
        _generator        = provider.Get<IMeshGenerator>();
        _clusterDataStore = provider.Get<IChunkClusterDataStore>();

        _signalBus = provider.Get<IEventBus>();

        _activeRegions.Load   += RenderRegion;
        _activeRegions.Unload += UnrenderRegion;

        _signalBus.Subscribe<LoadedRegionDataSignal>(OnRegionLoaded);
        _signalBus.Subscribe<ModifiedChunkDataSignal>(OnChunkUpdated);
    }

    private void RenderRegion(RegionCoord coord)
    {
        for (int y = -WorldConstants.REGION_NEG_CHUNKS; y < WorldConstants.REGION_POS_CHUNKS; y++)
        {
            var cCoord = new ChunkCoord(coord, y);

            if(_activeChunks.Add(cCoord))
                _ = GenerateMesh(cCoord);
        }
    }

    private void UnrenderRegion(RegionCoord coord)
    {
        for (int y = -WorldConstants.REGION_NEG_CHUNKS; y < WorldConstants.REGION_POS_CHUNKS; y++)
        {
            var cCoord = new ChunkCoord(coord, y);
            _activeChunks.Remove(cCoord);
            _renderSystem.UnloadChunk(cCoord);
        }
    }

    private void OnRegionLoaded(LoadedRegionDataSignal signal)
    {
        for (int r_x = -1; r_x <= 1; r_x++)
        {
            for (int r_z = -1; r_z <= 1; r_z++)
            {
                for (int y = -WorldConstants.REGION_NEG_CHUNKS; y < WorldConstants.REGION_POS_CHUNKS; y++)
                {
                    var cCoord = new ChunkCoord(signal.Coord + new RegionCoord(r_x, r_z), y);
                    _ = GenerateMesh(cCoord);
                }
            }
        }
    }

    private void OnChunkUpdated(ModifiedChunkDataSignal signal)
    {
        for (int c_x = -1; c_x <= 1; c_x++)
        {
            for (int c_y = -1; c_y <= 1; c_y++)
            {
                for (int c_z = -1; c_z <= 1; c_z++)
                {
                    var cCoord = new ChunkCoord(c_x, c_y, c_z) + signal.Coord;
                    _ = GenerateMesh(cCoord);
                }
            }
        }
    }

    private async Task GenerateMesh(ChunkCoord coord)
    {
        // Not loaded
        if (!_activeChunks.Contains(coord))
            return;

        // Already processing
        if (!_queue.Add(coord))
            return;

        await _semaphore.WaitAsync();

        // Back to the main thread
        await MainThreadDispatcher.Yield();

        GenerationData genData = null!;

        try
        {
            _queue.Remove(coord);

            if (!_dataPool.TryRequest(out genData))
                throw new Exception("Unable to attain required data from pool");

            bool success = await _clusterDataStore.GetChunkClusterDataAsync(coord, genData.DataArray, cts.Token);

            // Back to the main thread
            await MainThreadDispatcher.Yield();

            if (!success)
            {
                // Data can't exist so exit? Should never run
                _activeChunks.Remove(coord);
                return;
            }

            var cluster = genData.ClusterData; 
            cluster.SetData(genData.DataArray);

            await Task.Run(() =>
            {
                genData.Clear();
                _generator.GenerateMesh(cluster, genData.Opaque, genData.Transparent);
            });

            // Back to the main thread
            await MainThreadDispatcher.Yield();

            // Been removed since we started generating
            if (!_activeChunks.Contains(coord))
                return;

            _renderSystem.UpdateChunk(coord, genData.Opaque, genData.Transparent);
        }
        finally
        {
            if (genData != null)
                _dataPool.Return(genData);

            _semaphore.Release();
        }
    }

    public void Tick()
    {
        _activeRegions.Tick();
        DebugVariables.MeshQueueCount.Value = _queue.Count;
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<LoadedRegionDataSignal>(OnRegionLoaded);
        _signalBus.Unsubscribe<ModifiedChunkDataSignal>(OnChunkUpdated);

        cts.Cancel();
        cts.Dispose();
        GC.SuppressFinalize(this);
    }

    ~MeshGenerationSystem() => Dispose();

    private class GenerationData
    {
        public IChunkDataCluster ClusterData = new ChunkDataClusterFull();
        public ChunkData[] DataArray = new ChunkData[27];
        public VoxelMeshData Opaque = new();
        public VoxelMeshData Transparent = new();

        public void Clear()
        {
            Opaque.Clear();
            Transparent.Clear();
        }
    }
}
