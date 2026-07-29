using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;
using System.Diagnostics;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement;

public class Region
{
    public StateType CurrentState { get; private set; }
    public StateType TargetState { get; private set; }
    public bool IsTransitioning => _transition != null;

    private readonly RegionCoord _coord;
    private readonly TransitionController _transitions;

    private ITransition<StateType>? _transition;

    public Region(TransitionController transitions, RegionCoord coord)
    {
        _transitions = transitions;
        _coord = coord;
    }

    public void SetTargetState(StateType targetState)
    {
        TargetState = targetState;

        if (_transition != null && !_transition.IsCancelled)
        {
            bool isPromoting = _transition.Target > CurrentState;
            bool rqPromoting = TargetState > CurrentState;

            if (isPromoting != rqPromoting)
            {
                //_transition.Cancel();
            }
        }
    }

    public void Tick()
    {
        if (_transition == null)
        {
            if (CurrentState != TargetState)
                _transition = NextTransition();
            
            return;
        }

        if (_transition.IsComplete(out var newState))
        {
            CurrentState = newState;
            _transition = null;
        }
    }

    private ITransition<StateType>? NextTransition()
    {
        if (TargetState == CurrentState)
            return null;

        int direction = TargetState > CurrentState ? 1 : -1;
        ITransitionFactory factory = _transitions.Request(CurrentState, CurrentState + direction);

        return factory.GetTransition(_coord);
    }
}