using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Utils;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal class ThreadedTerrainGenerationSystem : ITerrainGenerationSystem
{
    const int MAX_UPDATES_PER_TICK = 16;

    readonly IWorldData worldData;

    readonly WorkerThread workerThread;

    readonly ConcurrentQueue<RegionCoord> toGenerate = new();
    readonly ConcurrentQueue<(RegionCoord coord, RegionData data)> complete = new();

    public ThreadedTerrainGenerationSystem(IWorldData worldData, ITerrainGenerator generator)
    {
        this.worldData = worldData;

        workerThread = new WorkerThread(() =>
        {
            while (toGenerate.TryDequeue(out RegionCoord coord))
            {
                var data = generator.Generate(coord);
                complete.Enqueue((coord, data));
            }
        });
    }

    public void Request(RegionCoord coord)
    {
        toGenerate.Enqueue(coord);
    }

    public void Tick()
    {
        for (int i = 0; i < MAX_UPDATES_PER_TICK && complete.TryDequeue(out var pair); i++)
        {
            worldData.SetRegion(pair.coord, pair.data);
        }
    }
}
