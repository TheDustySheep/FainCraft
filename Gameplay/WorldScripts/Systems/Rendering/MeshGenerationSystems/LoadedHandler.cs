using FainCraft.Gameplay.WorldScripts.Coords;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;

public class LoadedHandler
{
    private readonly IActiveRegionRadius _activeRegions;
    private readonly Action<RegionCoord, bool> _updateFunc;
    private readonly Action<RegionCoord> _unloadFunc;

    private readonly HashSet<RegionCoord> _loadedRegions = new();

    public LoadedHandler(
        IActiveRegionRadius activeRegions, 
        Action<RegionCoord, bool> updateFunc, 
        Action<RegionCoord> unloadFunc)
    {
        _activeRegions = activeRegions;
        _updateFunc = updateFunc;
        _unloadFunc = unloadFunc;

        SubscribeSignals();
    }

    ~LoadedHandler()
    {
        UnsubscribeSignals();
    }

    private void SubscribeSignals()
    {
        _activeRegions.Load   += OnRegionLoaded;
        _activeRegions.Unload += OnRegionUnloaded;
    }

    private void UnsubscribeSignals()
    {
        _activeRegions.Load   -= OnRegionLoaded;
        _activeRegions.Unload -= OnRegionUnloaded;
    }

    public bool IsLoaded(RegionCoord rCoord)
    {
        return _loadedRegions.Contains(rCoord);
    }

    private void OnRegionLoaded(RegionCoord rCoord)
    {
        // No action if already loaded
        if (!_loadedRegions.Add(rCoord))
            return;

        _updateFunc.Invoke(rCoord, true);

        foreach (var orCoord in WorldConstants.Iterate_Neighbour_Regions())
            _updateFunc.Invoke(rCoord + orCoord, false);
    }

    private void OnRegionUnloaded(RegionCoord rCoord)
    {
        // No action if already unloaded
        if (!_loadedRegions.Remove(rCoord))
            return;

        _unloadFunc.Invoke(rCoord);

        // Update neighbours
        foreach (var orCoord in WorldConstants.Iterate_Neighbour_Regions())
            _updateFunc.Invoke(rCoord + orCoord, false);
    }
}
