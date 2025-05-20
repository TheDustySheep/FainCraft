using FainCraft.Gameplay.PlayerScripts;
using FainEngine_v2.Utils.Variables;
using System.Collections.Concurrent;
using System.Diagnostics;
using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Files;

namespace FainCraft.Gameplay.OldWorldScripts.Systems.RegionLoading;

/*
internal class RegionLoadingController : IRegionLoadingController
{
    private readonly ReferenceVariable<PlayerPosition> _playerPosition = SharedVariables.PlayerPosition;
    private readonly IWorldData _worldData;
    private readonly ITerrainGenerationSystem _terrainGenerator;
    private readonly IFileLoadingSystem _fileLoader;

    // Per-region lock to prevent concurrent load/save on the same region
    private readonly ConcurrentDictionary<RegionCoord, SemaphoreSlim> _regionLocks = new();

    // Actions to execute on main thread
    private static readonly ConcurrentQueue<Action> _actions = new();

    public RegionLoadingController(
        IWorldData worldData,
        ITerrainGenerationSystem terrainGenerator,
        IFileLoadingSystem fileLoader)
    {
        _worldData = worldData;
        _terrainGenerator = terrainGenerator;
        _fileLoader = fileLoader;
    }

    public void Load(RegionCoord coord)
    {
        // If already loaded, no action
        if (_worldData.RegionExists(coord))
            return;

        // Run load in background, serialized per region
        _ = Task.Run(async () =>
        {
            var sem = _regionLocks.GetOrAdd(coord, _ => new SemaphoreSlim(1, 1));
            await sem.WaitAsync();
            try
            {
                // Attempt file load
                var data = await _fileLoader.LoadAsync(coord);
                if (data != null)
                {
                    // Queue set region on main thread
                    _actions.Enqueue(() => _worldData.SetRegion(coord, data));
                    return;
                }

                // Fallback generate
                var generationResult = await _terrainGenerator.GenerateAsync(coord, CancellationToken.None);
                if (generationResult != null)
                {
                    // Queue apply on main thread
                    _actions.Enqueue(() =>
                    {
                        _worldData.SetRegion(generationResult.RegionCoord, generationResult.RegionData);
                        _worldData.AddRegionEdits(generationResult.VoxelEdits);
                    });

                    // Persist newly generated region
                    await _fileLoader.SaveAsync(generationResult.RegionCoord, generationResult.RegionData);
                }
            }
            finally
            {
                sem.Release();
            }
        });
    }

    public void Unload(RegionCoord coord)
    {
        // Perform world unload immediately on main thread
        if (_worldData.UnloadRegion(coord, out var data))
        {
            // Save in background
            _ = Task.Run(async () =>
            {
                var sem = _regionLocks.GetOrAdd(coord, _ => new SemaphoreSlim(1, 1));
                await sem.WaitAsync();
                try
                {
                    await _fileLoader.SaveAsync(coord, data);
                }
                finally
                {
                    sem.Release();
                }
            });
        }
    }

    // Call this from the main update loop to apply load results
    public void Tick()
    {
        int i = 0;
        while (_actions.TryDequeue(out var action) && i < 1)
        {
            action.Invoke();
            i++;
        }
    }
}
*/