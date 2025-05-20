using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Signals;
using FainCraft.Gameplay.WorldScripts.Systems.Loading;
using FainEngine_v2.Utils;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Storage;

public class RegionDataStore : IRegionDataStore
{
    IEventBus _eventBus;
    IRegionDataLoader _loader;
    ConcurrentDictionary<RegionCoord, RegionData> _regions = new();

    public RegionDataStore(IServiceProvider serviceProvider)
    {
        _eventBus = serviceProvider.Get<IEventBus>();
        _loader   = serviceProvider.Get<IRegionDataLoader>();
    }

    public bool GetRegion(RegionCoord coord, out RegionData data)
    {
        return _regions.TryGetValue(coord, out data!);
    }

    public bool SetRegion(RegionCoord coord, RegionData data)
    {
        _regions[coord] = data;
        _eventBus.Publish(new LoadedRegionDataSignal()
        {
            Coord = coord,
            Data = data,
        });
        return true;
    }

    public bool EditRegion(RegionCoord coord, Func<RegionData, bool> func)
    {
        if (!_regions.TryGetValue(coord, out var data))
            return false;

        if (!func.Invoke(data))
            return false;

        _eventBus.Publish(new ModifiedRegionDataSignal()
        {
            Coord = coord,
            Data = data,
        });
        return true;
    }

    public async Task<RegionData?> GetRegionAsync(RegionCoord coord, CancellationToken token)
    {
        if (GetRegion(coord, out var data))
            return data;

        // Load the region
        var result = await _loader.GetRegionAsync(coord, token);

        // Return to the main thread
        await MainThreadDispatcher.Yield();

        _eventBus.Publish(new LoadedRegionDataSignal()
        {
            Coord = coord,
            Data = data,
        });

        // Already exists - maybe loaded it elsewhere
        if (_regions.TryGetValue(coord, out data))
            return data;

        _regions[coord] = result;
        return result;
    }
}
