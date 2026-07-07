namespace FainCraft.Signals.Gameplay.WorldScripts
{
    public static class DebugGenerationTimeSignals
    {
        public static event Action<TimeSpan>? OnMeshGenerate;
        public static event Action<TimeSpan>? OnTerrianGenerate;

        public static void MeshGenerate(TimeSpan time) => OnMeshGenerate?.Invoke(time);
        public static void TerrainGenerate(TimeSpan time) => OnTerrianGenerate?.Invoke(time);
    }
}
