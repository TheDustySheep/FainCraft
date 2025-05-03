using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration;
using FainEngine_v2.Extensions;
using FainEngine_v2.Utils.Variables;
using Silk.NET.Core.Native;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        // TODO Handle unloading from world data???


    }

    public void Tick()
    {
        // Handle file loaded data
        foreach ((var coord, var data) in _fileLoader.GetComplete())
        {
            // If the promise didn't exist something must have gone wrong - Discard anyway
            if (!_promises.TryGetValue(coord, out var promise))
                throw new Exception($"Promise for file loading did not exist {coord}");

            // Cancelled - Discard
            if (promise.IsCancelled)
                continue;

            // Handle the loaded data        
            _worldData.SetRegion(coord, data);
        }

        // Handle terrain generated data
        foreach (var result in _terrainGenerator.GetComplete())
        {
            // If the promise didn't exist something must have gone wrong - Discard anyway
            if (!_promises.TryGetValue(result.RegionCoord, out var promise))
                throw new Exception($"Promise for terrain generation did not exist {result.RegionCoord}");

            // Cancelled - Discard
            // TODO - Need to handle saving the generated data instead of discarding
            if (promise.IsCancelled)
                continue;

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
