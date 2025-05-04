namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering
{
    public struct RenderSettings
    {
        public int LoadRadius;
        public int RenderRadius;

        public static readonly RenderSettings Default = new()
        {
            LoadRadius = 4,
            RenderRadius = 2,
        };
    }
}
