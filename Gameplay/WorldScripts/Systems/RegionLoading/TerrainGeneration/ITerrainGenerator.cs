using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration;
internal interface ITerrainGenerator
{
    public RegionGenerationResult Generate(RegionCoord coord);
}
