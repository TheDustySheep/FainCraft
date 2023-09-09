using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Collections;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts;
internal abstract class AGenerationSystem<T_Item, T_Gen>
{
    // Callback
    Action<ChunkCoord, T_Item>? onComplete;

    // Generators
    readonly Queue<T_Gen> freeGenerators = new();
    readonly Dictionary<ChunkCoord, T_Gen> activeGenerators = new();

    // Requests
    readonly OrderedSet<ChunkCoord> requests = new();
    readonly ConcurrentQueue<(ChunkCoord, T_Item, T_Gen)> results = new();

    public AGenerationSystem(IEnumerable<T_Gen> generators, Action<ChunkCoord, T_Item>? onComplete)
    {
        this.onComplete = onComplete;

        foreach (var generator in generators)
        {
            freeGenerators.Enqueue(generator);
        }
    }

    public void RegisterCallback(Action<ChunkCoord, T_Item>? onComplete)
    {
        this.onComplete = onComplete;
    }

    public void Request(ChunkCoord coord)
    {
        requests.AddLast(coord);
    }

    public void Cancel(ChunkCoord coord)
    {
        requests.Remove(coord);
        activeGenerators.Remove(coord);
    }

    public void Update()
    {
        while (results.TryDequeue(out var pair))
        {
            if (activeGenerators.Remove(pair.Item1))
                onComplete?.Invoke(pair.Item1, pair.Item2);

            freeGenerators.Enqueue(pair.Item3);
        }

        while (requests.Count > 0 && freeGenerators.Count > 0)
        {
            var request = requests.Dequeue();
            var generator = freeGenerators.Dequeue();
            activeGenerators.Add(request, generator);
            Process(request, generator);
        }
    }

    private async void Process(ChunkCoord coord, T_Gen generator)
    {
        T_Item result = await Generate(coord, generator);
        results.Enqueue((coord, result, generator));
    }

    protected abstract Task<T_Item> Generate(ChunkCoord coord, T_Gen generator);
}
