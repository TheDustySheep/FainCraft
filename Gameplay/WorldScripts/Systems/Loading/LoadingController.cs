using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading;

internal class LoadingController : ILoadingController
{
    readonly IWorldData world;
    readonly ITerrainGenerationSystem terrainSystem;

    readonly HashSet<RegionCoord> regionsToLoad = new();
    readonly HashSet<RegionCoord> regionsToUnload = new();

    public LoadingController(IWorldData world, ITerrainGenerationSystem terrainSystem)
    {
        this.world = world;
        this.terrainSystem = terrainSystem;
    }

    public void OnLoad(RegionCoord coord)
    {
        regionsToLoad.Add(coord);
    }

    public void OnUnload(RegionCoord coord)
    {
        regionsToUnload.Add(coord);
    }

    public void Tick()
    {
        if (regionsToLoad.Any())
        {
            var coord = regionsToLoad.First();
            regionsToLoad.Remove(coord);

            // Check if loaded already
            if (world.GetRegion(coord) is not null)
                return;

            // Check if stored on disk

            // Generate new region
            terrainSystem.Request(coord);
        }
    }
}
