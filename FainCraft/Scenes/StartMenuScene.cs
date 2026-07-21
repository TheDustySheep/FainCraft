using FainCraft.UI;
using FainEngine_v2.Core;
using FainEngine_v2.UI.Fss;
using FainEngine_v2.UI.Fss.Parsing.Stylesheets;
using FainEngine_v2.UI.Rendering;
using FainEngine_v2.Utils;

namespace FainCraft.Scenes;

public class StartMenuScene : IScene
{
    EntityManager? _entityManager = null;
    IGameInputs _gameInputs;
    SceneManager _sceneManager;

    public StartMenuScene()
    {
        _gameInputs = DependencyInjector.Resolve<IGameInputs>();
        _sceneManager = DependencyInjector.Resolve<SceneManager>();
    }

    public void OnLoad()
    {
        _entityManager = new EntityManager();

        var sheetLoader = DependencyInjector.Resolve<IStylesheetLoader>();

        IReadOnlyList<FssClass> classes = sheetLoader.LoadFromFile("Resources/UIElements/Styles.fss");

        var uiRenderer = _entityManager.Spawn(new UICanvasRenderer(new StartMenuPanel()));

        uiRenderer.Canvas.AddClasses(classes);

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
