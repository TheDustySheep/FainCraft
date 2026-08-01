namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems
{
    public struct RenderSettings
    {
        // Rendering
        public uint RenderRadius;
        public uint MeshesAppliedPerFrame;
        public uint MeshingThreads;

        // Lighting
        public uint LightingUpdatesPerTick;

        // Loading
        public uint LoadRadius;

        public static readonly RenderSettings Default = new()
        {
            LoadRadius = 16,
            RenderRadius = 12,
            MeshesAppliedPerFrame = 4,
            LightingUpdatesPerTick = 4,
            MeshingThreads = 2,
        };
    }
}
