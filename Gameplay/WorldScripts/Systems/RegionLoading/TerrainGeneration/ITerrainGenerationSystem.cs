using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration;
internal interface ITerrainGenerationSystem
{
    public void Request(RegionCoord coord);
    public IEnumerable<RegionGenerationResult> GetComplete();
}
