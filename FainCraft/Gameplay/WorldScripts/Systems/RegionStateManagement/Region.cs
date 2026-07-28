using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement;

public class Region
{
    public StateType CurrentState { get; private set; }
    public StateType TargetState { get; private set; }

    private readonly ITransitionProvider _transitionProvider;
    private readonly RegionCoord _coord;

    private readonly object _lock = new();

    private bool _isProcessing = false;
    private CancellationTokenSource? _cts;

    public Region(ITransitionProvider transitionProvider, RegionCoord coord)
    {
        _transitionProvider = transitionProvider;
        _coord = coord;
    }

    /// <summary>
    /// Checks if anything is processing. If safe to delete returns true
    /// </summary>
    /// <returns>Safe to delete</returns>
    public bool CanBeDeleted()
    {
        lock (_lock)
        {
            return
                CurrentState == StateType.Unloaded &&
                TargetState  == StateType.Unloaded &&
                !_isProcessing;
        }
    }

    /// <summary>
    /// Sets the target state
    /// </summary>
    /// <param name="targetState">Desired target state</param>
    public void SetTargetState(StateType targetState)
    {
        lock (_lock)
        {
            if (targetState == TargetState)
                return; // No changes to target

            var oldTarget = TargetState;
            TargetState = targetState;

            if (_isProcessing)
            {
                // Already moving in the correct direction
                if ((oldTarget > CurrentState && TargetState > CurrentState) ||
                    (oldTarget < CurrentState && TargetState < CurrentState))
                    return;

                // Cancel the current transition
                _cts?.Cancel();
                return;
            }

            _isProcessing = true;
        }

        _ = Task.Run(ProcessTransition);
    }

    /// <summary>
    /// Internally handles the transition between states
    /// </summary>
    private async ValueTask ProcessTransition()
    {
        try
        {
            while (true)
            {
                TransitionAction transition;
                CancellationToken token;

                lock (_lock)
                {
                    if (CurrentState == TargetState)
                    {
                        _isProcessing = false;
                        return;
                    }

                    transition = _transitionProvider.GetTransition(CurrentState, TargetState);
                    _cts = new CancellationTokenSource();
                    token = _cts.Token;
                }

                try
                {
                    var outcome = await transition.Invoke(_coord, token);

                    lock (_lock)
                    {
                        CurrentState = outcome;
                    }
                }
                catch (OperationCanceledException) { }
                finally
                {
                    lock (_lock)
                    {
                        _cts?.Dispose();
                        _cts = null;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during transition: {ex}");
        }
        finally
        {
            lock (_lock)
            {
                _isProcessing = false;
            }
        }
    }
}
