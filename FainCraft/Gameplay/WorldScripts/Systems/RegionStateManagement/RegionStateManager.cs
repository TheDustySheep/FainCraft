using FainCraft.Gameplay.WorldScripts.Coords;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement;

public class RegionStateManager
{
    private readonly Dictionary<RegionCoord, Region> _regions = new();

    private IActiveRegionRadius _radius;
    private ITransitionProvider _transitions;

    public RegionStateManager(IActiveRegionRadius radius, ITransitionProvider transitions)
    {
        _radius = radius;
        _transitions = transitions;

        radius.Load += OnLoad;
        radius.Unload += OnUnload;
    }

    private void OnLoad(RegionCoord coord)
    {
        if (!_regions.TryGetValue(coord, out var region))
        {
            region = new Region(_transitions, coord);
            _regions.Add(coord, region);
        }

        region.SetTargetState(StateType.Meshed);
    }

    private void OnUnload(RegionCoord coord)
    {
        if (_regions.TryGetValue(coord, out var region))
            region.SetTargetState(StateType.Unloaded);
    }

    public void Tick()
    {
        _radius.Calculate();
        _regions.RemoveAll((k, v) => v.CanBeDeleted());
    }
}
