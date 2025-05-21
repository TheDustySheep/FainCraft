using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading;

public class SingleplayerDataLoader : IRegionDataLoader
{
    private readonly ITerrainGenerationSystem _generator;
    private readonly SemaphoreSlim _semaphore = new(4); // Max 4 concurrent
    private int _waitingCount = 0;

    public SingleplayerDataLoader(IServiceProvider provider)
    {
        _generator = provider.Get<ITerrainGenerationSystem>();
    }

    public async Task<RegionData> LoadRegionAsync(RegionCoord coord, CancellationToken token)
    {
        Interlocked.Increment(ref _waitingCount);
        await _semaphore.WaitAsync(token);
        Interlocked.Decrement(ref _waitingCount);

        DebugVariables.TerrainQueueCount.Value = _waitingCount;

        try
        {
            var result = await _generator.GenerateAsync(coord, token);
            return result.RegionData;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}