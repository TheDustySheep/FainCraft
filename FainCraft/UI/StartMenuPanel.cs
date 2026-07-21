using FainCraft.Scenes;
using FainEngine_v2.UI.UIElements;
using FainEngine_v2.UI.UIElements.Types;
using FainEngine_v2.Utils;

namespace FainCraft.UI;

internal class StartMenuPanel : UIElement
{
    SceneManager _sceneManager;
    public StartMenuPanel()
    {
        _sceneManager = DependencyInjector.Resolve<SceneManager>();

        var startMenuElement = DependencyInjector.Resolve<IUIElementLoader>().LoadFile("Resources/UIElements/StartMenu.xml");
        if (startMenuElement is not null)
            AddChild(startMenuElement);

        this.Query().FilterID( "new-game-button").First().Events.OnMouseClicked += () => _sceneManager.LoadScene(new SingleplayerWorld());
        this.Query().FilterID("load-game-button").First().Events.OnMouseClicked += () => _sceneManager.LoadScene(new SingleplayerWorld());
    }
}
