using System.Diagnostics.CodeAnalysis;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement.Transitions;

public interface ITransition<T>
{
    public T Target { get; } 
    public bool IsCancelled { get; }
    public bool IsComplete([NotNullWhen(true)] out T result);
    public void Cancel();
}