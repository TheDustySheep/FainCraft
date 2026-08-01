using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;
using FainEngine_v2.Utils;
using System.Diagnostics;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement;

public class RegionStateManager
{
    private readonly Dictionary<RegionCoord, Region> _regions = new();
    //private readonly List<Region> _inProgress = new();

    private readonly IActiveRegionRadius _radius;
    private readonly TransitionController _transitions;

    public RegionStateManager(IActiveRegionRadius radius, TransitionController transitions)
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

        _regions.RemoveAll((k, v) => !v.IsTransitioning && v.CurrentState == StateType.Unloaded);
    }
}
