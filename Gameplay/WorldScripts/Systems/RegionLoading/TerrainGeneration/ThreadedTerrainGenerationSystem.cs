using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Signals.Gameplay.WorldScripts;
using FainEngine_v2.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration;
internal class ThreadedTerrainGenerationSystem : ITerrainGenerationSystem
{
    readonly WorkerThread workerThread;

    readonly ConcurrentQueue<RegionGenerationResult> complete = new();

    readonly TerrainQueue _queue = new();

    public ThreadedTerrainGenerationSystem(ITerrainGenerator generator)
    {
        workerThread = new WorkerThread("Terrain Generation Thread 1", () =>
        {
            var sw = new Stopwatch();

            foreach (var coord in _queue.Dequeue(SharedVariables.PlayerPosition.Value.RegionCoord, 64))
            {
                sw.Restart();
                var data = generator.Generate(coord);
                complete.Enqueue(data);
                sw.Stop();
                DebugGenerationTimeSignals.TerrainGenerate(sw.Elapsed);
                DebugGenerationTimeSignals.TerrainQueueUpdate((uint)_queue.Count);
            }
        });
    }

    ~ThreadedTerrainGenerationSystem()
    {
        workerThread.Terminate();
    }

    public void Request(RegionCoord coord)
    {
        _queue.Enqueue(coord);
    }

    public IEnumerable<RegionGenerationResult> GetComplete()
    {
        while (complete.TryDequeue(out var result))
        {
            yield return result;
        }
    }
}
