using FainCraft.Gameplay;
using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts;
using FainCraft.UI;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.PostProcessing;
using FainEngine_v2.Resources;
using System.Numerics;

namespace FainCraft;
internal class FainCraftGameEngine : FainGameEngine
{
    public FainCraftGameEngine() : base(1600, 900, windowTitle: "FainCraft")
    {
    }

    protected override void Load()
    {

        var postShader  = ResourceLoader.LoadShader(@"C:\Users\Sean\source\repos\FainCraft\Resources\Shaders\PostProcessing\");
        var postProcess = EntityManager.SpawnEntity(new PostProcess(postShader));

        var ui = EntityManager.SpawnEntity<UIController>();
        ui.Canvas.Root.AddChild(new DebugStatsUI(ui.Canvas));


        SharedVariables.PlayerPosition.Value = new PlayerPosition(new Vector3(-300, 0, 100));
        var world = EntityManager.SpawnEntity<World>();
        var player = EntityManager.SpawnEntity(new PlayerEntity(world));
    }
}
