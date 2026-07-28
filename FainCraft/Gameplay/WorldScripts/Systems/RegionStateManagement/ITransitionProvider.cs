namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement;

public interface ITransitionProvider
{
    public TransitionAction GetTransition(StateType initial, StateType final);
}
