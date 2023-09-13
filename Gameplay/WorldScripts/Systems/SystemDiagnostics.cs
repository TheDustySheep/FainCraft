using FainEngine_v2.Core;
using FainEngine_v2.Entities;

namespace FainCraft.Gameplay.WorldScripts.Systems;
internal class SystemDiagnostics : IEntity
{
    float lastUpdate = 0f;
    readonly float updateFrequency = 1f;

    public void Update()
    {
        if (GameTime.TotalTime < lastUpdate + updateFrequency)
            return;

        lastUpdate = GameTime.TotalTime;

        Console.WriteLine("");
        Console.WriteLine("System Diagnostics");
        ReportTerrainGeneration();
        ReportMeshGeneration();
        Console.WriteLine("");
    }

    private static TimeSpan totalTerrainTime = TimeSpan.Zero;
    private static int totalTerrainGenerations = 0;
    public static void SubmitTerrainGeneration(TimeSpan timeSpan)
    {
        totalTerrainTime += timeSpan;
        totalTerrainGenerations++;
    }

    public static void ReportTerrainGeneration()
    {
        Console.WriteLine($"Terrain Generation Total Time: {totalTerrainTime.TotalMilliseconds:N} ms Total Count: {totalTerrainGenerations:N} Average: {totalTerrainTime.TotalMilliseconds / totalTerrainGenerations:N} ms");
    }

    private static TimeSpan totalMeshTime = TimeSpan.Zero;
    private static int totalMeshGenerations = 0;
    public static void SubmitMeshGeneration(TimeSpan timeSpan)
    {
        totalMeshTime += timeSpan;
        totalMeshGenerations++;
    }

    public static void ReportMeshGeneration()
    {
        Console.WriteLine($"Mesh Generation    Total Time: {totalMeshTime.TotalMilliseconds:N} ms Total Count: {totalMeshGenerations:N} Average: {totalMeshTime.TotalMilliseconds / totalMeshGenerations:N} ms");
    }
}