using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Editing;
using FainEngine_v2.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal class ThreadedTerrainGenerationSystem : ITerrainGenerationSystem
{
    const int MAX_UPDATES_PER_TICK = 1024;

    readonly IWorldData worldData;

    readonly WorkerThread workerThread;
    readonly ConcurrentQueue<RegionCoord> toGenerate = new();
    readonly ConcurrentQueue<GenerateResult> complete = new();

    public ThreadedTerrainGenerationSystem(IWorldData worldData, ITerrainGenerator generator)
    {
        this.worldData = worldData;

        workerThread = new WorkerThread("Terrain Generation Thread", () =>
        {
            var sw = new Stopwatch();
            while (toGenerate.TryDequeue(out RegionCoord coord))
            {
                Console.WriteLine($"To Generate Count {toGenerate.Count}");
                sw.Restart();
                var data = generator.Generate(coord);

                complete.Enqueue(new GenerateResult(coord, data.RegionData, data.VoxelEdits));
                sw.Stop();
                data.TerrainDebugData.TotalTime = sw.Elapsed;
                SystemDiagnostics.SubmitTerrainGeneration(data.TerrainDebugData);
            }
        });
    }

    public void Request(RegionCoord coord)
    {
        toGenerate.Enqueue(coord);
    }

    public void Tick()
    {
        int i = 0;
        for (i = 0; i < MAX_UPDATES_PER_TICK && complete.TryDequeue(out var result); i++)
        {
            worldData.SetRegion(result.Coord, result.RegionData);
            worldData.AddRegionEdits(result.VoxelEdits);
        }
        if (i > 0)
            Console.WriteLine($"Regions Generated this frame {i}. Queue Count {complete.Count}");
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
