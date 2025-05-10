namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems
{
    public struct RenderSettings
    {
        public uint LoadRadius;
        public uint RenderRadius;
        public uint MeshesAppliedPerFrame;
        public uint MeshQueueLimit;
        public uint LightingUpdatesPerTick;

        public static readonly RenderSettings Default = new()
        {
            LoadRadius = 6,
            RenderRadius = 4,
            MeshesAppliedPerFrame = 16,
            MeshQueueLimit = 4,
            LightingUpdatesPerTick = 1,
        };
    }
}
