namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems
{
    public struct RenderSettings
    {
        public uint LoadRadius;
        public uint RenderRadius;
        public uint MeshesAppliedPerFrame;
        public uint MeshQueueLimit;

        public static readonly RenderSettings Default = new()
        {
            LoadRadius = 16,
            RenderRadius = 12,
            MeshesAppliedPerFrame = 16,
            MeshQueueLimit = 4,
        };
    }
}
