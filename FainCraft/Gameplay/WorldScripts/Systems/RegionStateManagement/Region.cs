using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;
using System.Diagnostics;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement;

public class Region
{
    public StateType CurrentState { get; private set; }
    public StateType TargetState { get; private set; }

    private readonly RegionCoord _coord;
    private readonly TransitionController _transitions;
    private ITransition<StateType>? _transition;
    public bool IsTransitioning => _transition != null;

    public Region(TransitionController transitions, RegionCoord coord)
    {
        _transitions = transitions;
        _coord = coord;
    }

    // Sets the main 
    public void SetTargetState(StateType targetState)
    {
        TargetState = targetState;
        NextTransition();
    }

    // Callback on transition completion
    private void OnTransitionComplete(StateType newState)
    {
        CurrentState = newState;
        _transition = null;
        NextTransition();
    }

    private void NextTransition()
    {
        // Already transitioning or no transition required
        if (_transition != null || TargetState == CurrentState)
            return;

        // Determine the direction to move
        int direction = TargetState > CurrentState ? 1 : -1;

        // Request the transition from factory
        _transition = _transitions
            .Request(CurrentState, CurrentState + direction)
            .GetTransition(_coord);

        // Execute the transition
        _transition.Execute(OnTransitionComplete);
    }
}