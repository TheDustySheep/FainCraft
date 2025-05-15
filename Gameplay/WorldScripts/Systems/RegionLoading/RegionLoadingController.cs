using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration;
using FainEngine_v2.Utils.Variables;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading;

internal class RegionLoadingController : IRegionLoadingController
{
    readonly IWorldData _worldData;
    readonly ITerrainGenerationSystem _terrainGenerator;
    readonly IFileLoadingSystem _fileLoader;

    readonly ReferenceVariable<PlayerPosition> _playerPosition;
    readonly Dictionary<RegionCoord, GeneratePromise> _promises = new();

    public RegionLoadingController(IWorldData world, ITerrainGenerationSystem terrainGenerator, IFileLoadingSystem fileLoader)
    {
        _playerPosition = SharedVariables.PlayerPosition;

        _worldData = world;
        _fileLoader = fileLoader;
        _terrainGenerator = terrainGenerator;
    }

    public void Load(RegionCoord coord)
    {
        // Already loading? Uncancel
        if (_promises.TryGetValue(coord, out var promise))
        {
            promise.IsCancelled = false;
            return;
        }

        // Already loaded?
        if (_worldData.RegionExists(coord))
            return;
        
        // Try loading from file
        if (_fileLoader.Request(coord))
        {
            _promises[coord] = new GeneratePromise(GeneratePromise.System.FileLoader);
            return;
        }

        // Fallback - Generate
        _terrainGenerator.Request(coord);
        _promises[coord] = new GeneratePromise(GeneratePromise.System.TerrainGenerator);
    }

    public void Unload(RegionCoord coord)
    {
        // Already loading? Cancel
        if (_promises.TryGetValue(coord, out var promise))
        {
            promise.IsCancelled = true;
            return;
        }

        // Unload the data from the world
        if (_worldData.UnloadRegion(coord, out var data))
            _fileLoader.Save(coord, data);
    }

    public void Tick()
    {
        // Handle file loaded data
        foreach ((var coord, var data) in _fileLoader.GetComplete())
        {
            // If the promise didn't exist something must have gone wrong - Discard anyway
            if (!_promises.Remove(coord, out var promise))
                throw new Exception($"Promise for file loading did not exist {coord}");

            // Cancelled - Discard
            if (promise.IsCancelled)
                continue;

            // Failed to load the data from file
            if (data is null)
            {
                // Fallback - Generate
                _terrainGenerator.Request(coord);
                _promises[coord] = new GeneratePromise(GeneratePromise.System.TerrainGenerator);
                continue;
            }

            // Handle the loaded data        
            _worldData.SetRegion(coord, data);
        }

        // Handle terrain generated data
        foreach (var result in _terrainGenerator.GetComplete())
        {
            // If the promise didn't exist something must have gone wrong - Discard anyway
            if (!_promises.Remove(result.RegionCoord, out var promise))
                throw new Exception($"Promise for terrain generation did not exist {result.RegionCoord}");

            // Cancelled - Discard
            if (promise.IsCancelled)
            {
                _fileLoader.Save(result.RegionCoord, result.RegionData);
                continue;
            }

            // Handle the loaded data
            _worldData.SetRegion(result.RegionCoord, result.RegionData);
            _worldData.AddRegionEdits(result.VoxelEdits);
        }
    }

    private class GeneratePromise(GeneratePromise.System systemType)
    {
        public bool IsCancelled = false;
        public System SystemType = systemType;

        public enum System
        {
            FileLoader,
            TerrainGenerator
        }
    }
}
