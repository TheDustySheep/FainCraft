using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering;
using FainEngine_v2.Collections;

namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
internal class BasicMeshGenerationSystem : IMeshGenerationSystem
{
    readonly IWorldData worldData;
    readonly IRenderSystem renderSystem;
    readonly IMeshGenerator generator;

    const int MAX_UPDATES_PER_TICK = 1;

    readonly ChunkDataCluster cluster = new();

    readonly OrderedSet<ChunkCoord> toGenerate = new();

    public BasicMeshGenerationSystem(IWorldData worldData, IRenderSystem renderSystem, IMeshGenerator generator)
    {
        this.worldData = worldData;
        this.renderSystem = renderSystem;
        this.generator = generator;

        worldData.OnChunkUpdate += Generate;
    }

    public void Generate(ChunkCoord coord, bool immediate = false)
    {
        if (immediate)
            toGenerate.AddFirst(coord);
        else
            toGenerate.AddLast(coord);
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
