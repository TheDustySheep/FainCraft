using FainCraft.Gameplay;
using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts;
using FainEngine_v2.Core;
using FainEngine_v2.Rendering.PostProcessing;
using FainEngine_v2.Resources;
using FainEngine_v2.UI.Rendering;
using System.Numerics;

namespace FainCraft.Scenes
{
    public class SingleplayerWorld : IScene
    {
        EntityManager _entityManager = new();
        PlayerEntity _player = null!;

        public void OnLoad()
        {
            var postShader = ResourceLoader.LoadShader(@"Resources\Shaders\PostProcessing\");
            var postProcess = _entityManager.Spawn(new PostProcess(postShader));

            //var ui = EntityManager.Spawn(new UIController());
            //ui.AddCanvas();
            //ui.Root.AddChild(new DebugStatsUI(ui.Canvas));


            SharedVariables.PlayerPosition.Value = new PlayerPosition(new Vector3(-300, 0, 100));
            var world = _entityManager.Spawn(new Level());
            var player = _entityManager.Spawn(new PlayerEntity(world));
            _entityManager.Spawn(new UICanvasRenderer());
        }

        public void Update()
        {
            _entityManager.Update();
        }

        public void FixedUpdate()
        {
            _entityManager.FixedUpdate();
        }

        public void OnUnload()
        {
            _entityManager.DespawnAll();
        }
    }
}
