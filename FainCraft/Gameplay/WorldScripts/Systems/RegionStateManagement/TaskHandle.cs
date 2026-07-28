namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement;

public class Transition<T> where T : struct, Enum
{
    private readonly Func<(bool IsRunning, T? Outcome)> _poll;

    public Transition(Func<(bool IsRunning, T? Outcome)> poll)
    {
        _poll = poll;
    }

    public IEnumerable<T?> Run()
    {
        while (true)
        {
            var (isRunning, outcome) = _poll();

            if (!isRunning)
            {
                yield return outcome;
                yield break;
            }

            yield return null;
        }
    }
}