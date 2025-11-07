using FainCraft.UI.Panels;
using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.UI.UIElements;

namespace FainCraft.Scenes;

public class StartMenuScene : IScene
{
    UIController? _ui;

    public void OnLoad()
    {
        _ui = EntityManager.SpawnEntity(new UIController());

        var element = UIElementLoader.LoadXMLFile("Resources/UIElements/BaseElement.xml");
        _ui.Canvas.ClassList = UIElementLoader.LoadFssFile("Resources/UIElements/Styles.fss");
        if (element != null)
            _ui.Root.AddChildren(element);
    }

    public void OnUnload()
    {
        _ui = null;
    }
}
