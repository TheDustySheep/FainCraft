using FainCraft.Scenes;
using FainEngine_v2.UI;
using FainEngine_v2.UI.Fss.Parsing.Stylesheets;
using FainEngine_v2.UI.UIElements;
using FainEngine_v2.Utils;

namespace FainCraft.UI;

internal class StartMenuController
{
    UIDocument _document;

    public StartMenuController(ICanvas canvas)
    {
        var sceneManager  = DependencyInjector.Resolve<SceneManager>();
        var stylesLoader  = DependencyInjector.Resolve<IStylesheetLoader>();
        var elementLoader = DependencyInjector.Resolve<IUIElementLoader>();

        _document = new UIDocument(
            elementLoader.LoadElement("StartMenu"),
            stylesLoader.LoadFromFile("Resources/UI/Styles.fss")
        );

        canvas.AddDocument(_document);

        var root = _document.Root;

        root.Query().FilterID( "new-game-button").First().Events.OnMouseClicked += () => sceneManager.LoadScene(new SingleplayerWorld());
        root.Query().FilterID("load-game-button").First().Events.OnMouseClicked += () => sceneManager.LoadScene(new SingleplayerWorld());
    }
}
