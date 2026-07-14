using FainEngine_v2.Core;
using FainEngine_v2.UI.Fss;
using FainEngine_v2.UI.Fss.Parsing;
using FainEngine_v2.UI.Rendering;
using FainEngine_v2.UI.UIElements;
using FainEngine_v2.UI.UIElements.Types;
using FainEngine_v2.Utils;

namespace FainCraft.Scenes;
internal class UITestScene : IScene
{
    EntityManager _entityManager = new();
    IGameInputs _gameInputs;

    public UITestScene()
    {
        _gameInputs = DependencyInjector.Resolve<IGameInputs>();
    }

    public void OnLoad()
    {
        _gameInputs.SetCursorMode(Silk.NET.Input.CursorMode.Normal);
        
        UIElementLoader loader = new UIElementLoader();
        UIElement? elem = loader.LoadFile("Resources/UIElements/PauseMenu.xml");
        if (elem == null)
            elem = new();

        var sheetLoader = DependencyInjector.Resolve<IStylesheetLoader>();

        IReadOnlyList<FssClass> classes = sheetLoader.LoadFromFile("Resources/UIElements/Styles.fss");

        var uiRenderer = _entityManager.Spawn(new UICanvasRenderer(elem));

        uiRenderer.Canvas.AddClasses(classes);
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
