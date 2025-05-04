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

    const int MAX_UPDATES_PER_TICK = 1024;
    readonly WorkerThread[] workerThreads;

    MeshQueue _queue = new();

    public ThreadedMeshGenerationSystem(IWorldData worldData, IRenderSystem renderSystem, Func<IMeshGenerator> generatorFactory)
    {
        _worldData = worldData;
        _renderSystem = renderSystem;

        workerThreads = CreateWorkers(1, generatorFactory);

        worldData.OnChunkUpdate += Generate;
    }

    ~ThreadedMeshGenerationSystem()
    {
        foreach (var worker in workerThreads)
            worker.Terminate();
    }

    private WorkerThread[] CreateWorkers(int count, Func<IMeshGenerator> generatorFactory)
    {
        WorkerThread[] threads = new WorkerThread[count];
        for (int i = 0; i < count; i++)
        {
            threads[i] = new WorkerThread($"Mesh Generation Thread {i+1}", () =>
            {
                Stopwatch sw = new Stopwatch();
                var generator = generatorFactory.Invoke();
                while (_queue.TryDequeueGeneration(out var coord, out var cluster, out var meshData))
                {
                    sw.Restart();
                    generator.GenerateMesh(cluster, meshData);
                    _queue.EnqueueComplete(coord, cluster, meshData);
                    sw.Stop();
                    DebugGenerationTimeSignals.MeshGenerate(sw.Elapsed);
                }
            });
        }
        return threads;
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
            _queue.ReturnMeshData(meshData);
        }
    }
}
