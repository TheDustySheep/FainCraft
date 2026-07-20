using FainCraft.Scenes;
using FainEngine_v2.Core;
using FainEngine_v2.UI.Fss.Parsing;
using FainEngine_v2.UI.Fss.Parsing.Selectors;
using FainEngine_v2.UI.Fss.Parsing.Stylesheets;
using FainEngine_v2.UI.UIElements;
using FainEngine_v2.Utils;

namespace FainCraft;
internal class FainCraftGame : FainGameEngine
{
    private SceneManager _sceneManager;

    public FainCraftGame() : base(1600, 900, windowTitle: "FainCraft")
    {
        DependencyInjector.RegisterSingleton<IStylesheetLoader>(
            new StylesheetLoader(
                new SelectorPipeline(),
                new StylesParser()
        ));

        DependencyInjector.RegisterSingleton<IUIElementLoader>(
            new UIElementLoader(
                new StylesParser()
            )
        );

        _sceneManager = DependencyInjector.RegisterSingleton(new SceneManager(new SceneFactory()));

    }

    protected override void Update()
    {
        base.Update();

        if (gameInputs!.IsKeyDown(Silk.NET.Input.Key.N))
            _sceneManager.LoadScene(new StartMenuScene());

        _sceneManager.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        _sceneManager.FixedUpdate();
    }

    protected override void Load()
    {
        _sceneManager.LoadScene(new UITestScene());
    }
}
