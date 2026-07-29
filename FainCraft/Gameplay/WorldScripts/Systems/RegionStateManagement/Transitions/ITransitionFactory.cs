using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;

public interface ITransitionFactory
{
    public ITransition<StateType> GetTransition(RegionCoord coord);
}
