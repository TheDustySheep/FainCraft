namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems
{
    public struct RenderSettings
    {
        public int LoadRadius;
        public int RenderRadius;
        public int MeshesAppliedPerFrame;

        public static readonly RenderSettings Default = new()
        {
            LoadRadius = 16,
            RenderRadius = 4,
            MeshesAppliedPerFrame = 16,
        };
    }
}
