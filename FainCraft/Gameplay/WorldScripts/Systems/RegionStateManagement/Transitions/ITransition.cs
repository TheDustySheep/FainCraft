using System.Diagnostics.CodeAnalysis;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;

public interface ITransition<T>
{
    public void Execute(Action<T> onComplete);
}