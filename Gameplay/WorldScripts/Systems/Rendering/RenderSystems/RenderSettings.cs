namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems
{
    public struct RenderSettings
    {
        public int LoadRadius;
        public int RenderRadius;
        public int MeshesAppliedPerFrame;

        public static readonly RenderSettings Default = new()
        {
            LoadRadius = 4,
            RenderRadius = 4,
            MeshesAppliedPerFrame = 4,
        };
    }
}
