using FainCraft.UI;
using FainEngine_v2.Core;
using FainEngine_v2.UI;
using FainEngine_v2.Utils;

namespace FainCraft.Scenes;

public class StartMenuScene : IScene
{
    EntityManager _entityManager;
    IGameInputs _gameInputs;
    SceneManager _sceneManager;

    ICanvas? _canvas;
    StartMenuController? _controller;

    public StartMenuScene()
    {
        _gameInputs = DependencyInjector.Resolve<IGameInputs>();
        _sceneManager = DependencyInjector.Resolve<SceneManager>();

        _entityManager = new EntityManager();
    }

    public void OnLoad()
    {
        _canvas = _entityManager.Spawn(new ScreenCanvas());
        _controller = new StartMenuController(_canvas);

        _gameInputs.SetCursorMode(Silk.NET.Input.CursorMode.Normal);
    }

    public void Update()
    {
        _entityManager?.Update();
    }

    public void FixedUpdate()
    {
        _entityManager?.FixedUpdate();
    }

    public void OnUnload()
    {
        _entityManager?.DespawnAll();
    }
}
