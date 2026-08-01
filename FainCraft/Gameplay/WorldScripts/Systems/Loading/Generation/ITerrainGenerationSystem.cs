using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;
public interface ITerrainGenerationSystem
{
    public Task<RegionData> GenerateAsync(RegionCoord coord);
}
