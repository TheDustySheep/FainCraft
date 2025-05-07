using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
using FainCraft.Signals.Gameplay.WorldScripts;
using FainEngine_v2.Utils;
using FainEngine_v2.Utils.Variables;
using System.Diagnostics;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
internal class ThreadedMeshGenerationSystem : IMeshGenerationSystem
{
    readonly IWorldData _worldData;
    readonly IRenderSystem _renderSystem;

    ReferenceVariable<RenderSettings> _settings;
    readonly WorkerThread[] workerThreads;

    MeshQueue _queue = new();

    public ThreadedMeshGenerationSystem(IWorldData worldData, IRenderSystem renderSystem, Func<IMeshGenerator> generatorFactory)
    {
        _settings = SharedVariables.RenderSettings;
        _worldData = worldData;
        _renderSystem = renderSystem;

        workerThreads = CreateWorkers(1, generatorFactory);

        worldData.OnChunkUpdate += Generate;
    }

    ~ThreadedMeshGenerationSystem()
    {
        foreach (var worker in workerThreads)
            worker.Dispose();
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
                while (_queue.TryDequeueGeneration(out var coord, out var cluster, out var meshData1, out var meshData2))
                {
                    sw.Restart();
                    generator.GenerateMesh(cluster, meshData1, meshData2);
                    _queue.EnqueueComplete(coord, cluster, meshData1, meshData2);
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
        DebugVariables.MeshQueueCount.Value = _queue.BufferCount;

        // Handle requests
        for (int i = 0; i < _settings.Value.MeshesAppliedPerFrame && _queue.TryDequeueRequest(out var coord, out var cluster); i++)
        {
            cluster.SetData(_worldData.GetCluster(coord));
            _queue.EnqueueGeneration(coord, cluster);
        }

        // Handle completed
        for (int i = 0; i < _settings.Value.MeshesAppliedPerFrame && _queue.TryDequeueComplete(out var coord, out var opaque, out var transparent); i++)
        {
            _renderSystem.UpdateChunk(coord, opaque, transparent);
            _queue.ReturnMeshData(opaque);
            _queue.ReturnMeshData(transparent);
        }
    }
}
