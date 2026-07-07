using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;
internal interface ITerrainGenerator
{
    public RegionGenerationResult Generate(RegionCoord coord);
}
