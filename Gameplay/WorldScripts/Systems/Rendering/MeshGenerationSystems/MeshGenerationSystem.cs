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
    // Logic Handlers
    private readonly IActiveRegionRadius _loadedRadius;
    private readonly LoadedHandler _loadedHandler;
    private readonly RequestHandler _requestHandler;
    private readonly MeshGenerationQueue _queue;


    private readonly ObjectPoolFactory<GenerationData> _dataPool;
    private readonly ISignalBus _signalBus;
    private readonly IRenderSystem _renderSystem;
    private readonly IChunkClusterDataStore _clusterDataStore;

    private readonly int _maxPerFrame;
    private readonly int _maxActive;

    public MeshGenerationSystem(IServiceProvider provider, IActiveRegionRadius activeRegions)
    {
        // Constants
        _maxPerFrame = (int)SharedVariables.RenderSettings.Value.MeshesAppliedPerFrame;
        _maxActive   = (int)SharedVariables.RenderSettings.Value.MaxConcurrentMeshes;

        // Dependencies
        _signalBus        = provider.Get<ISignalBus>();
        _renderSystem     = provider.Get<IRenderSystem>();
        _clusterDataStore = provider.Get<IChunkClusterDataStore>();

        // Logic Handlers
        _loadedRadius = new ActiveRegionRadius(
            () => SharedVariables.RenderSettings.Value.RenderRadius
        );

        _requestHandler = new RequestHandler(
            _signalBus, 
            UpdateMesh,
            UpdateMesh
        );

        _loadedHandler = new LoadedHandler(
            _loadedRadius,
            UpdateMesh,
            UnloadRegion
        );

        _queue = new MeshGenerationQueue(_maxActive);
        _dataPool = new ObjectPoolFactory<GenerationData>(
            () => new GenerationData(provider.Get<IMeshGenerator>())
        );

        _queue.Start();
    }

    private static int CalculatePriority(RegionCoord coord, bool isNew)
    {
        int newBias = isNew ? 4 : 1000;
        int dstBias = (int)SharedVariables.PlayerPosition.Value.RegionCoord.ManhattenDistance(coord);
        return Math.Min(newBias, dstBias);
    }

    private void UpdateMesh(RegionCoord rCoord, bool isNew)
    {
        if (!_loadedHandler.IsLoaded(rCoord))
            return;

        int priority = CalculatePriority(rCoord, isNew);
        foreach (var y in WorldConstants.Iterate_Y_Chunks())
        {
            _queue.EnqueueRequest(new ChunkCoord(rCoord, y), priority);
        }
    }

    private void UpdateMesh(ChunkCoord cCoord, bool isNew)
    {
        RegionCoord rCoord = (RegionCoord)cCoord;
        if (!_loadedHandler.IsLoaded(rCoord))
            return;

        int priority = CalculatePriority(rCoord, isNew);
        _queue.EnqueueRequest(cCoord, priority);
    }

    private void UnloadRegion(RegionCoord rCoord) => _renderSystem.Unload(rCoord);

    public void Tick()
    {
        HandleUpdatingLoadedRegions();

        HandleNewRequests();

        HandleApplyingMeshes();
    }

    private void HandleUpdatingLoadedRegions()
    {
        // Update what should be loaded
        _loadedRadius.Calculate();
        DebugVariables.MeshQueueCount.Value = _queue.Count;
    }

    private void HandleApplyingMeshes()
    {
        int applied = 0;
        while (applied < _maxPerFrame && _queue.TryDequeueCompleted(out var item))
        {
            if (_loadedHandler.IsLoaded((RegionCoord)item.coord))
            {
                _renderSystem.UpdateMesh(item.coord, item.data.Opaque, item.data.Transparent);
                applied++;
            }

            _dataPool.Return(item.data);
            _queue.ReleaseSlot();
        }
    }

    private void HandleNewRequests()
    {
        // Gather data and enqueue new requests
        int dispatched = 0;
        while (dispatched < _maxPerFrame && _queue.TryDequeueRequest(out var coord))
        {
            // Requests was unloaded since being queued
            if (!_loadedHandler.IsLoaded((RegionCoord)coord))
            {
                _queue.ReleaseSlot();
                continue;
            }

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
    }

    public void Dispose()
    {
        _queue.Dispose();
        GC.SuppressFinalize(this);
    }

    ~MeshGenerationSystem() => Dispose();
}