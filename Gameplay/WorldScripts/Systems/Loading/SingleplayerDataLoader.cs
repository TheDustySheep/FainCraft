using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading;

public class SingleplayerDataLoader : IRegionDataLoader
{
    private readonly ITerrainGenerationSystem _generator;

    public SingleplayerDataLoader(IServiceProvider provider)
    {
        _generator = provider.Get<ITerrainGenerationSystem>();
    }

    public async Task<RegionData> GetRegionAsync(RegionCoord coord, CancellationToken token)
    {
        var result = await _generator.GenerateAsync(coord, token);

        return result.RegionData;
    }
}
