using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;

public class TransitionFactory : ITransitionFactory
{
    Func<RegionCoord, ITransition<StateType>> _factory;

    public TransitionFactory(Func<RegionCoord, ITransition<StateType>> factory)
    {
        _factory = factory;
    }

    public ITransition<StateType> GetTransition(RegionCoord coord)
    {
        return _factory.Invoke(coord);
    }
}
