using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal class BasicTerrainGenerationSystem : ITerrainGenerationSystem
{
    const int MAX_UPDATES_PER_TICK = 4;

    readonly IWorldData worldData;
    readonly ITerrainGenerator generator;

    readonly Queue<RegionCoord> toGenerate = new();

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
