using FainCraft.UI.Panels;
using FainEngine_v2.Core;
using FainEngine_v2.Entities;

namespace FainCraft.Scenes
{
    public class StartMenuScene : IScene
    {
        UIController? _ui;

        public void OnLoad()
        {
            _ui = EntityManager.SpawnEntity<UIController>();
            _ui.Root.AddChildren(new StartMenuUIPanel(_ui.Canvas));
        }

        public void OnUnload()
        {
            _ui = null;
        }
    }
}
