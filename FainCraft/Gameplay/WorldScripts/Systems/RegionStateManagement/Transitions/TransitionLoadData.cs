using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Storage;
using FainCraft.Gameplay.WorldScripts.Systems.Loading;
using System.Diagnostics.CodeAnalysis;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;

public class TransitionLoadData : ITransition<StateType>
{
    private readonly IRegionDataStore  _store;
    private readonly IRegionDataLoader _system;

    private bool _isRunning = true;
    private StateType _outcome;

    private CancellationTokenSource _cts;
    private readonly Task _task;

    public bool IsCancelled { get; private set; }

    public StateType Target => StateType.Loaded;

    public TransitionLoadData(IRegionDataLoader system, IRegionDataStore store, RegionCoord coord)
    {
        _store = store;
        _system = system;
        _cts = new();
        _task = Run(coord);
    }

    private async Task Run(RegionCoord coord)
    {
        var data = await _system.LoadRegionAsync(coord, _cts.Token);

        if (data is not null)
        {
            _store.SetRegion(coord, data);
            _outcome = StateType.Loaded;
        }
        else if (_store.GetRegion(coord, out data))
            _outcome = StateType.Loaded;
        else
            _outcome = StateType.Unloaded;

        _isRunning = false;
    }

    public bool IsComplete([NotNullWhen(true)] out StateType result)
    {
        if (_isRunning)
        {
            result = default;
            return false;
        }

        result = _outcome;
        return true;
    }

    public void Cancel()
    {
        if (IsCancelled)
            return;

        IsCancelled = true;
        _cts.Cancel();
    }
}

public class TransitionUnloadData : ITransition<StateType>
{
    private readonly IRegionDataStore _store;
    private readonly IRegionDataLoader _system;

    private bool _isRunning = true;
    private StateType _outcome;

    private CancellationTokenSource _cts;
    private readonly Task _task;

    public bool IsCancelled => _cts.IsCancellationRequested;

    public StateType Target => StateType.Loaded;

    public TransitionUnloadData(IRegionDataLoader system, IRegionDataStore store, RegionCoord coord)
    {
        _store = store;
        _system = system;
        _cts = new();
        _task = Run(coord);
    }

    private async Task Run(RegionCoord coord)
    {
        if (!_store.GetRegion(coord, out var data))
        {
            _outcome = StateType.Unloaded;
            _isRunning = false;
            return;
        }

        _store.SetRegion(coord, null);

        await _system.UnloadRegionAsync(coord, data);

        _outcome = StateType.Unloaded;
        _isRunning = false;
    }

    public bool IsComplete([NotNullWhen(true)] out StateType result)
    {
        if (_isRunning)
        {
            result = default;
            return false;
        }

        result = _outcome;
        return true;
    }

    public void Cancel()
    {
        _cts.Cancel();
    }
}