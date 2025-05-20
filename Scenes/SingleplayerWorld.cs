using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.OldWorldScripts;
using FainCraft.Gameplay;
using FainCraft.UI;
using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.PostProcessing;
using FainEngine_v2.Resources;
using System.Numerics;
using FainCraft.UI.Overlays;
using FainCraft.Gameplay.WorldScripts;

namespace FainCraft.Scenes
{
    public class SingleplayerWorld : IScene
    {

        PlayerEntity _player = null!;

        public void OnLoad()
        {
            var postShader = ResourceLoader.LoadShader(@"Resources\Shaders\PostProcessing\");
            var postProcess = EntityManager.SpawnEntity(new PostProcess(postShader));

            var ui = EntityManager.SpawnEntity<UIController>();
            ui.Root.AddChild(new DebugStatsUI(ui.Canvas));


            SharedVariables.PlayerPosition.Value = new PlayerPosition(new Vector3(-300, 0, 100));
            var world = EntityManager.SpawnEntity(new Level());
            var player = EntityManager.SpawnEntity(new PlayerEntity(world));
        }

        public void OnUnload()
        {

        }
    }
}
