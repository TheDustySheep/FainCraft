using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Editing;
using FainEngine_v2.Utils;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal class ThreadedTerrainGenerationSystem : ITerrainGenerationSystem
{
    const int MAX_UPDATES_PER_TICK = 16;

    readonly IWorldData worldData;

    readonly WorkerThread workerThread;
    readonly ConcurrentQueue<RegionCoord> toGenerate = new();
    readonly ConcurrentQueue<GenerateResult> complete = new();

    public ThreadedTerrainGenerationSystem(IWorldData worldData, ITerrainGenerator generator)
    {
        this.worldData = worldData;

        workerThread = new WorkerThread(() =>
        {
            while (toGenerate.TryDequeue(out RegionCoord coord))
            {
                var data = generator.Generate(coord);
                complete.Enqueue(new GenerateResult(coord, data.RegionData, data.VoxelEdits.ToArray()));
            }
        });
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
            worldData.AddVoxelEdits(result.VoxelEdits);
        }
    }

    private class GenerateResult
    {
        public RegionCoord Coord;
        public RegionData RegionData;
        public IEnumerable<IVoxelEdit> VoxelEdits;

        public GenerateResult(RegionCoord coord, RegionData regionData, IEnumerable<IVoxelEdit> edits)
        {
            Coord = coord;
            RegionData = regionData;
            VoxelEdits = edits;
        }
    }
}
