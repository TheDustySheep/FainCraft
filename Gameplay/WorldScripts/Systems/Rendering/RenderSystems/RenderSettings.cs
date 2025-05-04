namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems
{
    public struct RenderSettings
    {
        public int LoadRadius;
        public int RenderRadius;

        public static readonly RenderSettings Default = new()
        {
            LoadRadius = 16,
            RenderRadius = 12,
        };
    }
}
