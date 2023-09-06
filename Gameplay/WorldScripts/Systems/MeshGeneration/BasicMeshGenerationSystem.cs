using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering;
using FainEngine_v2.Collections;
using FainEngine_v2.Utils;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
internal class BasicMeshGenerationSystem : IMeshGenerationSystem
{
    IWorldData worldData;
    IRenderSystem renderSystem;
    IMeshGenerator generator;

    const int MAX_UPDATES_PER_TICK = 1;

    ChunkDataCluster cluster = new();

    HashQueue<ChunkCoord> toGenerate = new();

    public BasicMeshGenerationSystem(IWorldData worldData, IRenderSystem renderSystem, IMeshGenerator generator)
    {
        this.worldData = worldData;
        this.renderSystem = renderSystem;
        this.generator = generator;

        worldData.OnChunkUpdate += Generate;
    }

    public void Generate(ChunkCoord coord)
    {
        toGenerate.Enqueue(coord);
    }

    public void Tick()
    {
        for (int i = 0; i < MAX_UPDATES_PER_TICK && toGenerate.TryDequeue(out var coord); i++)
        {
            cluster.SetData(worldData.GetCluster(coord));
            var data = generator.GenerateMesh(cluster);
            renderSystem.UpdateChunk(coord, data);
        }
    }
}
