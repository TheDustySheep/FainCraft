namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;

public class TransitionController
{
    private Dictionary<(StateType, StateType), ITransitionFactory> _factories = new();

    public void Register(StateType initial, StateType final, ITransitionFactory factory)
    {
        _factories[(initial, final)] = factory;
    }

    public ITransitionFactory Request(StateType initial, StateType final)
    {
        return _factories[(initial, final)];
    }
}
