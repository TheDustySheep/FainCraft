using FainCraft.UI;
using FainEngine_v2.Core;
using FainEngine_v2.UI.Rendering;
using FainEngine_v2.Utils;

namespace FainCraft.Scenes;
internal class UITestScene : IScene
{
    EntityManager _entityManager = new();
    IGameInputs _gameInputs;
    FPSPanel? _panel;

    public UITestScene()
    {
        _gameInputs = DependencyInjector.Resolve<IGameInputs>();
    }

    public void OnLoad()
    {
        _panel = new FPSPanel();
        _entityManager.Spawn(new UICanvasRenderer(_panel));
        _gameInputs.SetCursorMode(Silk.NET.Input.CursorMode.Normal);
    }

    public void Update()
    {
        _entityManager.Update();
        _panel.Update();
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
