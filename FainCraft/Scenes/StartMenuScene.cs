using FainCraft.UI.Panels;
using FainEngine_v2.Core;
using FainEngine_v2.Entities;

namespace FainCraft.Scenes;

public class StartMenuScene : IScene
{
    EntityManager? _entityManager = null;
    //UIController? _ui = null;

    public void OnLoad()
    {
        _entityManager = new EntityManager();
        //_ui = _entityManager.Spawn(new UIController());
        //
        //_ui.AddCanvas(
        //    UIElementLoader.LoadCanvas(
        //        "Resources/UIElements/BaseElement.xml",
        //        "Resources/UIElements/Styles.fss"
        //        )
        //    );
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
