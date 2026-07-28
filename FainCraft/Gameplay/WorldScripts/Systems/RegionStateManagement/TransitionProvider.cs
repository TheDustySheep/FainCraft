using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement;

public class TransitionProvider : ITransitionProvider
{
    private readonly TransitionAction[] _promotions;
    private readonly TransitionAction[] _demotions;

    public TransitionProvider(TransitionAction[] promotions, TransitionAction[] demotions)
    {
        _promotions = promotions;
        _demotions = demotions;
    }

    public TransitionAction GetTransition(StateType initial, StateType final)
    {
        if (initial == final)
            return (coord, token) => ValueTask.FromResult(final);

        if (final > initial)
            return _promotions[(int)initial];
        else
            return _demotions[(int)initial];
    }
}
