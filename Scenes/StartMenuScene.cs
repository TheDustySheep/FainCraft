using FainCraft.UI.Panels;
using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.UI.FontRendering;
using FainEngine_v2.UI.UIElements;

namespace FainCraft.Scenes;

public class StartMenuScene : IScene
{
    UIController? _ui;

    public void OnLoad()
    {
        _ui = EntityManager.SpawnEntity(new UIController());

        _ui.AddCanvas(
            UIElementLoader.LoadCanvas(
                "Resources/UIElements/BaseElement.xml",
                "Resources/UIElements/Styles.fss"
                )
            );
    }

    public void OnUnload()
    {
        _ui = null;
    }
}
