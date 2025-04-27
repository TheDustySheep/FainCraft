using CsvHelper;
using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.Utils;
using System.Globalization;

namespace FainCraft.Gameplay.WorldScripts.Systems;
internal class SystemDiagnostics : IEntity
{
    static readonly WriteOverQueue<MeshGenDebugData> MeshData = new(1024);
    static readonly WriteOverQueue<TerrainDebugData> TerrainTimes = new(128);

    Timer timer;

    public SystemDiagnostics()
    {
        timer = new Timer(PrintStats, null, 0, 1000);
    }

    public static void SubmitMeshGeneration(MeshGenDebugData data)
    {
        MeshData.Add(data);
    }

    public static void SubmitTerrainGeneration(TerrainDebugData data)
    {
        TerrainTimes.Add(data);
    }

    private void PrintStats(object? state)
    {
        Console.WriteLine($"=== Generation Stats ===");
        if (TerrainTimes.Count > 0)
        {
            var values = TerrainTimes.ToArray();
            var avg = values.Average(i => i.TotalTime.TotalMilliseconds);
            Console.WriteLine($" - Terrain Generation   {avg:F4}ms - {1d / avg:00000.0} R/ms ({WorldConstants.REGION_Y_TOTAL_COUNT} C/R)");
        }
        if (MeshData.Count > 0)
        {
            var values = MeshData.ToArray();
            var avg = values.Average(i => i.TotalTime.TotalMilliseconds);
            Console.WriteLine($" - Mesh Data Generation {avg:F4}ms - {1d / avg:00000.0} C/ms");
        }
    }

    public void Dispose()
    {
        timer.Dispose();

        try
        {
            using var writer = new StreamWriter("Resources\\Debug\\mesh_times.csv");
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(MeshData.ToArray());
        }
        catch { }
    }
}

internal struct MeshGenDebugData
{
    public TimeSpan TotalTime;
}

internal struct TerrainDebugData
{
    public TimeSpan TotalTime;
    public TimeSpan ChunkTime => TotalTime / WorldConstants.REGION_Y_TOTAL_COUNT;
}