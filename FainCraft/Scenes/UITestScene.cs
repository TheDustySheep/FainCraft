using FainEngine_v2.Core;
using FainEngine_v2.UI.Fss;
using FainEngine_v2.UI.Fss.Parsing.Stylesheets;
using FainEngine_v2.UI.UIElements;
using FainEngine_v2.UI.UIElements.Types;
using FainEngine_v2.Utils;
using Silk.NET.Windowing;

namespace FainCraft.Scenes;
internal class UITestScene : IScene
{
    EntityManager _entityManager = new();
    IGameInputs _gameInputs;
    IWindow _window;

    public UITestScene()
    {
        _gameInputs = DependencyInjector.Resolve<IGameInputs>();
        _window = DependencyInjector.Resolve<IWindow>();
    }

    public void OnLoad()
    {
        _gameInputs.SetCursorMode(Silk.NET.Input.CursorMode.Normal);

        var elem = LoadPauseMenuElement();

        var sheetLoader = DependencyInjector.Resolve<IStylesheetLoader>();

        IReadOnlyList<FssClass> classes = sheetLoader.LoadFromFile("Resources/UIElements/Styles.fss");

        //var uiRenderer = _entityManager.Spawn(new CanvasEntity(elem));

        //uiRenderer.Canvas.AddClasses(classes);
    }

    private UIElement LoadPauseMenuElement()
    {
        var loader = DependencyInjector.Resolve<IUIElementLoader>();

        UIElement? elem = loader.LoadElement("PauseMenu");
        if (elem == null)
            elem = new();

        elem.Query()
            .FilterID("resume-button")
            .First()
            .Events
            .OnMouseClicked += () => Console.WriteLine("Open resume menu");

        elem.Query()
            .FilterID("settings-button")
            .First()
            .Events
            .OnMouseClicked += () => Console.WriteLine("Open settings menu");

        elem.Query()
            .FilterID("exit-button")
            .First()
            .Events
            .OnMouseClicked += _window.Close;

        return elem;
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
