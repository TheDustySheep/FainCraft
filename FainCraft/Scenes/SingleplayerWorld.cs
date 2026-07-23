using FainCraft.Gameplay;
using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts;
using FainCraft.UI;
using FainEngine_v2.Core;
using FainEngine_v2.Rendering.PostProcessing;
using FainEngine_v2.Resources;
using FainEngine_v2.UI;
using FainEngine_v2.Utils;
using System.Numerics;

namespace FainCraft.Scenes
{
    public class SingleplayerWorld : IScene
    {
        EntityManager _entityManager = new();
        PlayerEntity _player = null!;

        readonly IGameInputs _gameInputs;

        public SingleplayerWorld()
        {
            _gameInputs = DependencyInjector.Resolve<IGameInputs>();
        }


        public void OnLoad()
        {
            _gameInputs.SetCursorMode(Silk.NET.Input.CursorMode.Raw);

            var postShader = ResourceLoader.LoadShader(@"Resources\Shaders\PostProcessing\");
            var postProcess = _entityManager.Spawn(new PostProcess(postShader));


            var canvas = _entityManager.Spawn(new ScreenCanvas());
            _entityManager.Spawn(new DebugPanelController(canvas));

            SharedVariables.PlayerPosition.Value = new PlayerPosition(new Vector3(-300, 0, 100));
            var world = _entityManager.Spawn(new Level());
            var player = _entityManager.Spawn(new PlayerEntity(world));

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
            _player = null!;
        }
    }
}
