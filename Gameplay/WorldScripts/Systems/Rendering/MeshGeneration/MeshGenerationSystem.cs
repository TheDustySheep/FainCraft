using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Clusters;
using FainCraft.Gameplay.WorldScripts.Signals;
using FainCraft.Gameplay.WorldScripts.Storage;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;
using FainEngine_v2.Collections;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;

/// <summary>
/// Manages mesh generation for chunks in a voxel world.
/// Uses a priority queue to order requests (regions vs. updates),
/// a semaphore to limit concurrent in-flight mesh objects,
/// and a background worker thread to perform the heavy mesh generation.
/// </summary>
internal class MeshGenerationSystem : IMeshGenerationSystem, IDisposable
{
    private readonly IEventBus _signalBus;
    private readonly IRenderSystem _renderSystem;
    private readonly IActiveRegionRadius _activeRegions;
    private readonly IChunkClusterDataStore _clusterDataStore;

    private readonly PriorityHashQueue<ChunkCoord, int> _requestQueue;
    private readonly ConcurrentQueue<GenerationData> _toGenerate = new();
    private readonly ConcurrentQueue<(ChunkCoord coord, GenerationData data)> _completed = new();

    private readonly int _maxPerFrame;
    private readonly int _maxActiveData;
    private readonly SemaphoreSlim _semaphore;
    private readonly CancellationTokenSource _cts = new();
    private readonly Thread _worker;
    private readonly ObjectPoolFactory<GenerationData> _dataPool;

    public MeshGenerationSystem(
        IServiceProvider provider,
        IActiveRegionRadius activeRegions)
    {
        _renderSystem = provider.Get<IRenderSystem>();
        _clusterDataStore = provider.Get<IChunkClusterDataStore>();
        _signalBus = provider.Get<IEventBus>();
        _activeRegions = activeRegions;

        _maxPerFrame = (int)SharedVariables.RenderSettings.Value.MeshesAppliedPerFrame;
        _maxActiveData = (int)SharedVariables.RenderSettings.Value.MaxConcurrentMeshes;

        _semaphore = new SemaphoreSlim(_maxActiveData, _maxActiveData);
        _requestQueue = new PriorityHashQueue<ChunkCoord, int>();

        _dataPool = new ObjectPoolFactory<GenerationData>(
            () => new GenerationData(provider.Get<IMeshGenerator>())
        );

        // Start worker thread
        _worker = new Thread(WorkerLoop) { IsBackground = true };
        _worker.Start();

        // Subscribe to region and chunk signals
        _activeRegions.Load += OnRegionLoaded;
        _activeRegions.Unload += OnRegionUnloaded;
        _signalBus.Subscribe<LoadedRegionDataSignal>(OnLoadedRegionData);
        _signalBus.Subscribe<ModifiedRegionDataSignal>(OnModified);
        _signalBus.Subscribe<ModifiedChunkDataSignal> (OnModified);
        _signalBus.Subscribe<ModifiedVoxelStateSignal>(OnModified);
    }

    private static int CalculatePriority(RegionCoord coord, bool isNew)
    {
        int newBias = isNew ? 4 : 1000;
        int dstBias = (int)SharedVariables.PlayerPosition.Value.RegionCoord.ManhattenDistance(coord);
        return Math.Min(newBias, dstBias);
    }

    // Region load: mark all chunks in region active and enqueue
    private void OnRegionLoaded(RegionCoord region)
    {
        for (int y = -WorldConstants.REGION_NEG_CHUNKS; y < WorldConstants.REGION_POS_CHUNKS; y++)
        {
            _renderSystem.Load(region);
            EnqueueRequest(new ChunkCoord(region, y), isNew: true);
        }
    }

    // Region unload: mark chunks inactive, unload and remove any pending requests
    private void OnRegionUnloaded(RegionCoord region)
    {
        for (int y = -WorldConstants.REGION_NEG_CHUNKS; y < WorldConstants.REGION_POS_CHUNKS; y++)
        {
            _renderSystem.Unload(region);
            _requestQueue.Remove(new ChunkCoord(region, y));
        }
    }

    // When underlying region data arrives, enqueue neighbors if still active
    private void OnLoadedRegionData(LoadedRegionDataSignal signal)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                var region = signal.Coord + new RegionCoord(dx, dz);
                if (!_renderSystem.Exists(region))
                    continue;

                for (int y = -WorldConstants.REGION_NEG_CHUNKS; y < WorldConstants.REGION_POS_CHUNKS; y++)
                {
                    var coord = new ChunkCoord(region, y);
                    EnqueueRequest(coord, isNew: true);
                }
            }
        }
    }

    #region Modified Callbacks
    // UpdateMesh neighbouring regions
    private void OnModified(ModifiedRegionDataSignal signal)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                for (int y = -WorldConstants.REGION_NEG_CHUNKS; y < WorldConstants.REGION_POS_CHUNKS; y++)
                {
                    var coord = (ChunkCoord)signal.Coord + new ChunkCoord(dx, y, dz);
                    if (_renderSystem.Exists((RegionCoord)coord))
                        EnqueueRequest(coord, isNew: false);
                }
            }
        }
    }

    // UpdateMesh neighbouring chunks
    private void OnModified(ModifiedChunkDataSignal signal)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    var coord = signal.Coord + new ChunkCoord(dx, dy, dz);
                    if (_renderSystem.Exists((RegionCoord)coord))
                        EnqueueRequest(coord, isNew: false);
                }
            }
        }
    }

    // UpdateMesh neighbouring chunks
    private void OnModified(ModifiedVoxelStateSignal signal)
    {
        ChunkCoord cCoord = (ChunkCoord)signal.Coord;

        // UpdateMesh self
        EnqueueRequest(cCoord, isNew: false);

        // UpdateMesh neighbours if touching
        foreach (var cOffset in GetTouchedNeighborOffsets(signal.Coord))
        {
            var coord = cCoord + cOffset;
            if (_renderSystem.Exists((RegionCoord)coord))
                EnqueueRequest(coord, isNew: false);
        }
    }

    /// <summary>
    /// Lazily yields (dx, dy, dz) offsets for every chunk that the voxel lies against,
    /// skipping the centre and allocating nothing except the compiler‑generated enumerator.
    /// Chunk size is assumed to be 32^3 (indices 0‥31).
    /// </summary>
    private static IEnumerable<ChunkCoord> GetTouchedNeighborOffsets(VoxelCoordGlobal v)
    {
        for (int dx = -1; dx <= 1; dx++)
        { 
            if (dx == -1 && v.X !=  0) continue;
            if (dx ==  1 && v.X != 31) continue;

            for (int dy = -1; dy <= 1; dy++)
            { 
                if (dy == -1 && v.Y !=  0) continue;
                if (dy ==  1 && v.Y != 31) continue;

                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dz == -1 && v.Z !=  0) continue;
                    if (dz ==  1 && v.Z != 31) continue;

                    // Skip the current chunk
                    if (dx == 0 && dy == 0 && dz == 0)
                        continue;

                    yield return new ChunkCoord(dx, dy, dz);
                }
            }
        }
    }
    #endregion

    private void EnqueueRequest(ChunkCoord coord, bool isNew)
    {
        if (!_renderSystem.Exists((RegionCoord)coord))
            return;

        int priority = CalculatePriority((RegionCoord)coord, isNew);
        _requestQueue.Enqueue(coord, priority);
    }

    private void WorkerLoop()
    {
        while (!_cts.IsCancellationRequested)
        {
            if (_toGenerate.TryDequeue(out var genData))
            {
                try
                {
                    genData.Generator.GenerateMesh(genData.ClusterData, genData.Opaque, genData.Transparent);
                    _completed.Enqueue((genData.Coord, genData));
                }
                catch
                {
                    _dataPool.Return(genData);
                    _semaphore.Release();
                }
            }
            else Thread.Sleep(1);
        }
    }

    public void Tick()
    {
        _activeRegions.Calculate();
        DebugVariables.MeshQueueCount.Value = _requestQueue.Count;

        int dispatched = 0;
        while (dispatched < _maxPerFrame
               && _requestQueue.TryDequeue(out var coord, out _)
               && _semaphore.Wait(0))
        {
            var gen = _dataPool.Request();
            if (!_clusterDataStore.GetChunkClusterData(coord, gen.DataArray))
            {
                _dataPool.Return(gen);
                _semaphore.Release();
                continue;
            }

            gen.Coord = coord;
            gen.ClusterData.SetData(gen.DataArray);
            _toGenerate.Enqueue(gen);
            dispatched++;
        }

        int applied = 0;
        while (applied < _maxPerFrame && _completed.TryDequeue(out var item))
        {
            if (_renderSystem.Exists((RegionCoord)item.coord))
            {
                _renderSystem.UpdateMesh(item.coord, item.data.Opaque, item.data.Transparent);
                applied++;
            }

            _dataPool.Return(item.data);
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _worker.Join();

        _activeRegions.Load -= OnRegionLoaded;
        _activeRegions.Unload -= OnRegionUnloaded;
        _signalBus.Unsubscribe<LoadedRegionDataSignal>(OnLoadedRegionData);
        _signalBus.Unsubscribe<ModifiedRegionDataSignal>(OnModified);
        _signalBus.Unsubscribe<ModifiedChunkDataSignal> (OnModified);
        _signalBus.Unsubscribe<ModifiedVoxelStateSignal>(OnModified);

        GC.SuppressFinalize(this);
    }

    ~MeshGenerationSystem() => Dispose();

    private class GenerationData
    {
        public ChunkCoord Coord;
        public IMeshGenerator Generator;
        public IChunkDataCluster ClusterData = new ChunkDataClusterFull();
        public ChunkData[] DataArray = new ChunkData[27];
        public VoxelMeshData Opaque = new();
        public VoxelMeshData Transparent = new();

        public GenerationData(IMeshGenerator generator)
        {
            Generator = generator;
        }
    }
}