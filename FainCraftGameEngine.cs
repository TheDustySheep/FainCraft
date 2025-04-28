using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts;
using FainCraft.Gameplay.WorldScripts.Systems;
using FainEngine_v2.Core;
using FainEngine_v2.Entities;
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
        EntityManager.SpawnEntity<SystemDiagnostics>();

        var postShader  = ResourceLoader.LoadShader(@"C:\Users\Sean\source\repos\FainCraft\Resources\Shaders\PostProcessing\");
        var postProcess = EntityManager.SpawnEntity(new PostProcess(postShader));

        //var ui = EntityManager.SpawnEntity<UIController>(); 

        var world = EntityManager.SpawnEntity<World>();
        var player = EntityManager.SpawnEntity(new PlayerEntity(world));
    }
}
