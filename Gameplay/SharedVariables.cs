using FainCraft.Gameplay.PlayerScripts;
using FainEngine_v2.Utils.Variables;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;

namespace FainCraft.Gameplay
{
    public static class SharedVariables
    {
        public static readonly ReferenceVariable<PlayerPosition> PlayerPosition = new();
        public static readonly ReferenceVariable<RenderSettings> RenderSettings = new(FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems.RenderSettings.Default);
    }
}
