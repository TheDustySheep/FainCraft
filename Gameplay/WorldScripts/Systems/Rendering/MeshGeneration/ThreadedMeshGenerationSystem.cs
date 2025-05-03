using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Signals.Gameplay.WorldScripts;
using FainEngine_v2.Utils;
using System.Diagnostics;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
internal class ThreadedMeshGenerationSystem : IMeshGenerationSystem
{
    readonly IWorldData _worldData;
    readonly IRenderSystem _renderSystem;

    const int MAX_UPDATES_PER_TICK = 16;
    readonly WorkerThread workerThread1;
    readonly WorkerThread workerThread2;

    MeshQueue _queue = new();

    public ThreadedMeshGenerationSystem(IWorldData worldData, IRenderSystem renderSystem, Func<IMeshGenerator> generatorFactory)
    {
        _worldData = worldData;
        _renderSystem = renderSystem;

        workerThread1 = CreateWorker(1, generatorFactory);
        workerThread2 = CreateWorker(2, generatorFactory);

        worldData.OnChunkUpdate += Generate;
    }

    ~ThreadedMeshGenerationSystem()
    {
        workerThread1.Terminate();
        workerThread2.Terminate();
    }

    private WorkerThread CreateWorker(int i, Func<IMeshGenerator> generatorFactory)
    {
        return new WorkerThread($"Mesh Generation Thread {i}", () =>
        {
            Stopwatch sw = new Stopwatch();
            var generator = generatorFactory.Invoke();
            while (_queue.TryDequeueGeneration(out var coord, out var cluster))
            {
                sw.Restart();
                var data = generator.GenerateMesh(cluster);
                _queue.EnqueueComplete(coord, cluster, data);
                sw.Stop();
                DebugGenerationTimeSignals.MeshGenerate(sw.Elapsed);
            }
        });
    }

    public void Generate(ChunkCoord coord, bool important)
    {
        _queue.EnqueueRequest(coord, important);
    }

    public void Tick()
    {
        DebugGenerationTimeSignals.MeshQueueUpdate((uint)_queue.QueueCount);

        // Handle requests
        for (int i = 0; i < MAX_UPDATES_PER_TICK; i++)
        {
            if (!_queue.TryDequeueRequest(out var coord, out var cluster))
                break;

            cluster.SetData(_worldData.GetCluster(coord));
            _queue.EnqueueGeneration(coord, cluster);
        }

        // Handle completed
        for (int i = 0; i < MAX_UPDATES_PER_TICK && _queue.TryDequeueComplete(out var coord, out var meshData); i++)
        {
            _renderSystem.UpdateChunk(coord, meshData);
        }
    }
}
