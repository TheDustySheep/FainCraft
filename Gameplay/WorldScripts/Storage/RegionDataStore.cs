using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Signals;

namespace FainCraft.Gameplay.WorldScripts.Storage;

public class RegionDataStore : IRegionDataStore
{
    IEventBus _eventBus;
    Dictionary<RegionCoord, RegionData> _regions = new();

    public RegionDataStore(IServiceProvider serviceProvider)
    {
        _eventBus = serviceProvider.Get<IEventBus>();
    }

    public bool GetRegion(RegionCoord coord, out RegionData data)
    {
        return _regions.TryGetValue(coord, out data!);
    }

    public bool SetRegion(RegionCoord coord, RegionData? data)
    {
        if (data == null)
        {
            if (!_regions.Remove(coord))
                return false;

            _eventBus.Publish(new UnloadedRegionDataSignal()
            {
                Coord = coord,
            });
        }
        else
        {
            _regions[coord] = data;
            _eventBus.Publish(new LoadedRegionDataSignal()
            {
                Coord = coord,
                Data = data,
            });
        }

        DebugVariables.WorldLoadedRegions.Value = _regions.Count;
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
}
