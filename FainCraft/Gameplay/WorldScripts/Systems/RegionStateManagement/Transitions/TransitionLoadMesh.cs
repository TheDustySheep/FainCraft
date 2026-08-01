using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;

public class TransitionLoadMesh : ITransition<StateType>
{
    private readonly RegionCoord _coord;
    private readonly IRenderingSystem _renderController;

    public TransitionLoadMesh(IRenderingSystem renderController, RegionCoord coord)
    {
        _coord = coord;
        _renderController = renderController;
    }

    public void Execute(Action<StateType> onComplete)
    {
        _renderController.Add(_coord);
        onComplete.Invoke(StateType.Meshed);
    }
}

public class TransitionUnloadMesh : ITransition<StateType>
{
    private readonly RegionCoord _coord;
    private readonly IRenderingSystem _renderController;

    public TransitionUnloadMesh(IRenderingSystem renderController, RegionCoord coord)
    {
        _coord = coord;
        _renderController = renderController;
    }

    public void Execute(Action<StateType> onComplete)
    {
        _renderController.Remove(_coord);
        onComplete.Invoke(StateType.Loaded);
    }
}
