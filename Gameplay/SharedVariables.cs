using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering;
using FainEngine_v2.Utils.Variables;

namespace FainCraft.Gameplay
{
    public static class SharedVariables
    {
        public static readonly ReferenceVariable<PlayerPosition> PlayerPosition = new();
        public static readonly ReferenceVariable<RenderSettings> RenderSettings = new(WorldScripts.Systems.Rendering.RenderSettings.Default);
    }
}
