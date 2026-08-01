using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading;

public class SingleplayerDataLoader : IRegionDataLoader
{
    private readonly ITerrainGenerationSystem _generator;
    private readonly SemaphoreSlim _semaphore = new(4); // Max 4 concurrent
    private int _waitingCount = 0;

    public SingleplayerDataLoader(ITerrainGenerationSystem generator)
    {
        _generator = generator;
    }

    public async Task<RegionData> LoadRegionAsync(RegionCoord coord)
    {
        Interlocked.Increment(ref _waitingCount);
        await _semaphore.WaitAsync();
        Interlocked.Decrement(ref _waitingCount);

        DebugVariables.TerrainQueueCount.Value = _waitingCount;

        try
        {
            return await _generator.GenerateAsync(coord);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task UnloadRegionAsync(RegionCoord coord, RegionData data)
    {
        return Task.CompletedTask;
    }
}