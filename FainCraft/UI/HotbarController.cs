using FainCraft.Scenes;
using FainEngine_v2.UI;
using FainEngine_v2.UI.Fss.Parsing.Stylesheets;
using FainEngine_v2.UI.UIElements;
using FainEngine_v2.Utils;

namespace FainCraft.UI;

internal class HotbarController
{
    UIDocument _document;

    public HotbarController(ICanvas canvas)
    {
        var sceneManager = DependencyInjector.Resolve<SceneManager>();
        var stylesLoader = DependencyInjector.Resolve<IStylesheetLoader>();
        var elementLoader = DependencyInjector.Resolve<IUIElementLoader>();

        _document = new UIDocument(
            elementLoader.LoadElement("Hotbar"),
            stylesLoader.LoadFromFile("Resources/UI/Hotbar/Hotbar.fss")
        );

        canvas.AddDocument(_document);

        var root = _document.Root;
    }
}
