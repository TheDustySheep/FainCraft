using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;
internal interface ITerrainGenerator
{
    public RegionData Generate(RegionCoord coord);
}
