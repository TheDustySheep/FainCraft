using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal interface ITerrainGenerator
{
    public RegionData Generate(RegionCoord coord);
}
