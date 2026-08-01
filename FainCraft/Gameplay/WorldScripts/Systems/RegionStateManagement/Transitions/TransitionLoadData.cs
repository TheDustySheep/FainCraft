using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Storage;
using FainCraft.Gameplay.WorldScripts.Systems.Loading;
using FainEngine_v2.Utils.Threading;
using System.Diagnostics.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;

public class TransitionLoadData : ITransition<StateType>
{
    private readonly RegionCoord _coord;
    private readonly IRegionDataStore  _store;
    private readonly IRegionDataLoader _system;

    public TransitionLoadData(IRegionDataLoader system, IRegionDataStore store, RegionCoord coord)
    {
        _coord = coord;
        _store = store;
        _system = system;
    }

    public void Execute(Action<StateType> onComplete)
    {
        Task.Run(async () =>
        {
            var data = await _system.LoadRegionAsync(_coord);

            MainThreadDispatcher.Enqueue(() =>
            {
                if (_store.SetRegion(_coord, data))
                    onComplete.Invoke(StateType.Loaded);
                else
                    onComplete.Invoke(StateType.Unloaded);
            });
        });
    }
}

public class TransitionUnloadData : ITransition<StateType>
{
    private readonly RegionCoord _coord;
    private readonly IRegionDataStore _store;
    private readonly IRegionDataLoader _system;

    public TransitionUnloadData(IRegionDataLoader system, IRegionDataStore store, RegionCoord coord)
    {
        _coord = coord;
        _store = store;
        _system = system;
    }

    public void Execute(Action<StateType> onComplete)
    {
        // If the region doesn't exist - nothing to unload
        if (!_store.GetRegion(_coord, out var data))
        {
            onComplete.Invoke(StateType.Unloaded);
            return;
        }

        // Empty the region
        _store.SetRegion(_coord, null);

        Task.Run(async () =>
        {
            await _system.UnloadRegionAsync(_coord, data);

            MainThreadDispatcher.Enqueue(() =>
            {
                onComplete.Invoke(StateType.Unloaded);
            });
        });
    }
}