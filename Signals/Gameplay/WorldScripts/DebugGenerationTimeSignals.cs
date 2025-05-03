namespace FainCraft.Signals.Gameplay.WorldScripts
{
    public static class DebugGenerationTimeSignals
    {
        public static event Action<TimeSpan>? OnMeshGenerate;
        public static event Action<uint>? OnMeshQueueUpdate;
        public static event Action<uint>? OnLoadedMeshesUpdate;

        public static event Action<TimeSpan>? OnTerrianGenerate;
        public static event Action<uint>? OnTerrainQueueUpdate;

        public static void MeshGenerate(TimeSpan time) => OnMeshGenerate?.Invoke(time);
        public static void MeshQueueUpdate(uint count) => OnMeshQueueUpdate?.Invoke(count);

        public static void TerrainGenerate(TimeSpan time) => OnTerrianGenerate?.Invoke(time);
        public static void TerrainQueueUpdate(uint count) => OnTerrainQueueUpdate?.Invoke(count);

        public static void LoadedMeshesCountUpdate(uint count) => OnLoadedMeshesUpdate?.Invoke(count);
    }
}
