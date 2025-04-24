using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal interface ITerrainGenerator
{
    internal RegionGenerationResult Generate(RegionCoord coord);
}
