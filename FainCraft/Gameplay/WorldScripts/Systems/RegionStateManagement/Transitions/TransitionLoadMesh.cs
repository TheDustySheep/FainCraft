using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;

public class TransitionLoadMesh : ITransition<StateType>
{
    public TransitionLoadMesh(IRenderingSystem renderController, RegionCoord coord)
    {
        renderController.Add(coord);
    }

    public StateType Target => StateType.Meshed;
    public bool IsCancelled => false;
    public void Cancel() { }

    public bool IsComplete([NotNullWhen(true)] out StateType result)
    {
        result = StateType.Meshed;
        return true;
    }
}

public class TransitionUnloadMesh : ITransition<StateType>
{
    public TransitionUnloadMesh(IRenderingSystem renderController, RegionCoord coord)
    {
        renderController.Remove(coord);
    }

    public StateType Target => StateType.Loaded;
    public bool IsCancelled => false;
    public void Cancel() { }

    public bool IsComplete([NotNullWhen(true)] out StateType result)
    {
        result = StateType.Loaded;
        return true;
    }
}
