using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering;
using FainEngine_v2.Collections;
using FainEngine_v2.Utils;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
internal class ThreadedMeshGenerationSystem : IMeshGenerationSystem
{
    readonly IWorldData worldData;
    readonly IRenderSystem renderSystem;
    readonly IMeshGenerator generator;

    const int MAX_UPDATES_PER_TICK = 16;
    readonly WorkerThread workerThread;

    readonly Queue<ChunkDataCluster> clusterPool = new Queue<ChunkDataCluster>();
    readonly OrderedSet<ChunkCoord> toGenerate = new();
    readonly ConcurrentQueue<(ChunkCoord coord, ChunkDataCluster cluster)> buffer = new();
    readonly ConcurrentQueue<(ChunkCoord coord, ChunkDataCluster cluster, VoxelMeshData data)> complete = new();

    public ThreadedMeshGenerationSystem(IWorldData worldData, IRenderSystem renderSystem, IMeshGenerator generator)
    {
        this.worldData = worldData;
        this.renderSystem = renderSystem;
        this.generator = generator;

        for (int i = 0; i < MAX_UPDATES_PER_TICK; i++)
        {
            clusterPool.Enqueue(new ChunkDataCluster());
        }

        workerThread = new WorkerThread(() =>
        {
            while (buffer.TryDequeue(out var pair))
            {
                var data = generator.GenerateMesh(pair.cluster);
                complete.Enqueue((pair.coord, pair.cluster, data));
            }
        });

        worldData.OnChunkUpdate += Generate;
    }

    public void Generate(ChunkCoord coord)
    {
        toGenerate.AddLast(coord);
    }

    public void Tick()
    {
        for (int i = 0; i < MAX_UPDATES_PER_TICK; i++)
        {
            if (clusterPool.Count == 0 || toGenerate.Count == 0)
                break;

            if (!clusterPool.TryDequeue(out var cluster))
                break;

            if (!toGenerate.TryDequeue(out var coord))
            {
                clusterPool.Enqueue(cluster);
                break;
            }

            cluster.SetData(worldData.GetCluster(coord));

            buffer.Enqueue((coord, cluster));
        }

        for (int i = 0; i < MAX_UPDATES_PER_TICK && complete.TryDequeue(out var pair); i++)
        {
            renderSystem.UpdateChunk(pair.coord, pair.data);
            clusterPool.Enqueue(pair.cluster);
        }
    }
}
