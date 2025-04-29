using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Editing;
using FainEngine_v2.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal class ThreadedTerrainGenerationSystem : ITerrainGenerationSystem
{
    const int MAX_UPDATES_PER_TICK = 1;

    readonly IWorldData worldData;

    readonly WorkerThread workerThread;

    readonly ConcurrentQueue<RegionCoord> toGenerate = new();
    readonly ConcurrentQueue<GenerateResult> complete = new();

    Timer timer;

    public ThreadedTerrainGenerationSystem(IWorldData worldData, ITerrainGenerator generator)
    {
        this.worldData = worldData;

        int count = 0;
        timer = new Timer(x =>
        {
            Console.WriteLine($"Generating {count} regions/sec");
            count = 0;
        }, null, 1000, 1000);

        workerThread = new WorkerThread("Terrain Generation Thread 1", () =>
        {
            var sw = new Stopwatch();

            while (toGenerate.TryDequeue(out RegionCoord coord))
            {
                sw.Restart();
                var data = generator.Generate(coord);
            
                complete.Enqueue(new GenerateResult(coord, data.RegionData, data.VoxelEdits));
                sw.Stop();
                SystemDiagnostics.SubmitTerrainGeneration(sw.Elapsed);
                count++;
            }
        });
    }

    ~ThreadedTerrainGenerationSystem()
    {
        workerThread.Terminate();
    }

    public void Request(RegionCoord coord)
    {
        toGenerate.Enqueue(coord);
    }

    public void Tick()
    {
        for (int i = 0; i < MAX_UPDATES_PER_TICK && complete.TryDequeue(out var result); i++)
        {
            worldData.SetRegion(result.Coord, result.RegionData);
            worldData.AddRegionEdits(result.VoxelEdits);
        }
    }

    private class GenerateResult
    {
        public RegionCoord Coord;
        public RegionData RegionData;
        public RegionEditList VoxelEdits;

        public GenerateResult(RegionCoord coord, RegionData regionData, RegionEditList edits)
        {
            Coord = coord;
            RegionData = regionData;
            VoxelEdits = edits;
        }
    }
}
