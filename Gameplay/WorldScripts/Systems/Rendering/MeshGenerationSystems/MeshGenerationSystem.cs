using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Signals;
using FainCraft.Gameplay.WorldScripts.Storage;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
using FainEngine_v2.Collections;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;

/// <summary>
/// Manages mesh generation for chunks in a voxel world.
/// Uses MeshGenerationQueue to handle requests, concurrency, and threading.
/// </summary>
internal class MeshGenerationSystem : IMeshGenerationSystem, IDisposable
{
    private readonly RequestHandler _requestHandler;
    private readonly MeshGenerationQueue _queue;
    private readonly ObjectPoolFactory<GenerationData> _dataPool;
    private readonly ISignalBus _signalBus;
    private readonly IRenderSystem _renderSystem;
    private readonly IActiveRegionRadius _activeRegions;
    private readonly IChunkClusterDataStore _clusterDataStore;
    private readonly int _maxPerFrame;

    private readonly HashSet<RegionCoord> _loadedRegions = new();

    public MeshGenerationSystem(IServiceProvider provider, IActiveRegionRadius activeRegions)
    {
        _renderSystem = provider.Get<IRenderSystem>();
        _clusterDataStore = provider.Get<IChunkClusterDataStore>();
        _signalBus = provider.Get<ISignalBus>();
        _activeRegions = activeRegions;

        _requestHandler = new RequestHandler(_signalBus, AddMeshRequest);

        _maxPerFrame = (int)SharedVariables.RenderSettings.Value.MeshesAppliedPerFrame;
        int maxActive = (int)SharedVariables.RenderSettings.Value.MaxConcurrentMeshes;

        _queue = new MeshGenerationQueue(maxActive);
        _dataPool = new ObjectPoolFactory<GenerationData>(
            () => new GenerationData(provider.Get<IMeshGenerator>())
        );

        SubscribeSignals();
        _queue.Start();
    }

    private void SubscribeSignals()
    {
        _activeRegions.Load += OnRegionLoaded;
        _activeRegions.Unload += OnRegionUnloaded;
    }

    private static int CalculatePriority(RegionCoord coord, bool isNew)
    {
        int newBias = isNew ? 4 : 1000;
        int dstBias = (int)SharedVariables.PlayerPosition.Value.RegionCoord.ManhattenDistance(coord);
        return Math.Min(newBias, dstBias);
    }

    private void AddMeshRequest(ChunkCoord coord, bool isNew)
    {
        RegionCoord rCoord = (RegionCoord)coord;
        if (!_loadedRegions.Contains(rCoord))
            return;

        int prio = CalculatePriority(rCoord, isNew);
        _queue.EnqueueRequest(coord, prio);
    }

    private void OnRegionLoaded(RegionCoord rCoord)
    {
        // Already added?
        if (!_loadedRegions.Add(rCoord))
            return;

        foreach (var c_y in WorldConstants.Iterate_Y_Chunks())
        {
            AddMeshRequest(new ChunkCoord(rCoord, c_y), true);
        }

        // Update neighbours
        foreach (var orCoord in WorldConstants.Iterate_Neighbour_Regions())
        {
            foreach (var c_y in WorldConstants.Iterate_Y_Chunks())
            {
                AddMeshRequest(new ChunkCoord(rCoord + orCoord, c_y), false);
            }
        }
    }

    private void OnRegionUnloaded(RegionCoord rCoord)
    {
        _renderSystem.Unload(rCoord);

        foreach (var c_y in WorldConstants.Iterate_Y_Chunks())
        {
            _queue.RemoveRequest(new ChunkCoord(rCoord, c_y));
        }

        // Update neighbours
        foreach (var orCoord in WorldConstants.Iterate_Neighbour_Regions())
        {
            foreach (var c_y in WorldConstants.Iterate_Y_Chunks())
            {
                AddMeshRequest(new ChunkCoord(rCoord + orCoord, c_y), false);
            }
        }
    }

    public void Tick()
    {
        _activeRegions.Calculate();
        DebugVariables.MeshQueueCount.Value = _queue.Count;

        int dispatched = 0;
        while (dispatched < _maxPerFrame &&
               _queue.TryDequeueRequest(out var coord) &&
               _queue.TryWaitForSlot())
        {
            var gen = _dataPool.Request();
            if (!_clusterDataStore.GetChunkClusterData(coord, gen.DataArray))
            {
                _dataPool.Return(gen);
                _queue.ReleaseSlot();
                continue;
            }
            gen.Coord = coord;
            gen.ClusterData.SetData(gen.DataArray);
            _queue.EnqueueToGenerate(gen);
            dispatched++;
        }

        int applied = 0;
        while (applied < _maxPerFrame && _queue.TryDequeueCompleted(out var item))
        {
            if (_loadedRegions.Contains((RegionCoord)item.coord))
            {
                _renderSystem.UpdateMesh(item.coord, item.data.Opaque, item.data.Transparent);
                applied++;
            }
            _dataPool.Return(item.data);
            _queue.ReleaseSlot();
        }
    }

    public void Dispose()
    {
        _queue.Dispose();
        UnsubscribeSignals();
        GC.SuppressFinalize(this);
    }

    private void UnsubscribeSignals()
    {
        _activeRegions.Load -= OnRegionLoaded;
        _activeRegions.Unload -= OnRegionUnloaded;
    }

    ~MeshGenerationSystem() => Dispose();
}