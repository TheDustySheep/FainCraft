using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading;

public interface IRegionDataLoader
{
    public Task<RegionData> GetRegionAsync(RegionCoord coord, CancellationToken token);
}
