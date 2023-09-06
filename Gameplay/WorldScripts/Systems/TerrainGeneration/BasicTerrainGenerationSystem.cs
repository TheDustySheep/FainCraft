using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Utils;
using System.Collections.Concurrent;
using System.Numerics;
using System.Reflection.Emit;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal class BasicTerrainGenerationSystem : ITerrainGenerationSystem
{
    const int MAX_UPDATES_PER_TICK = 4;

    IWorldData worldData;
    ITerrainGenerator generator;

    Queue<RegionCoord> toGenerate = new();

    public BasicTerrainGenerationSystem(IWorldData worldData, ITerrainGenerator generator)
    {
        this.worldData = worldData;
        this.generator = generator;
    }

    public void Request(RegionCoord coord)
    {
        toGenerate.Enqueue(coord);
    }

    public void Tick()
    {
        for (int i = 0; i < MAX_UPDATES_PER_TICK && toGenerate.TryDequeue(out RegionCoord coord); i++)
        {
            var data = generator.Generate(coord);
            worldData.SetRegion(coord, data);
        }
    }
}
