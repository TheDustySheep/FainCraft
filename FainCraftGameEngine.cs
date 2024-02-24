using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts;
using FainEngine_v2.Core;
using FainEngine_v2.Rendering.PostProcessing;
using FainEngine_v2.Resources;

namespace FainCraft;
internal class FainCraftGameEngine : FainGameEngine
{
    public FainCraftGameEngine() : base(1600, 900, windowTitle: "FainCraft")
    {
    }

    protected override void Load()
    {
        //EntityManager.SpawnEntity<SystemDiagnostics>();

        var post_shader = ResourceLoader.LoadShader(@"C:\Users\Sean\source\repos\FainCraft\Resources\Shaders\PostProcessing\");
        var ui = EntityManager.SpawnEntity(new PostProcess(post_shader));
        var world = EntityManager.SpawnEntity<World>();
        var player = EntityManager.SpawnEntity(new PlayerEntity(world));
    }
}
