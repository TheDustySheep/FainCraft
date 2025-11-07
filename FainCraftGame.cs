using FainCraft.Scenes;
using FainEngine_v2.Core;

namespace FainCraft;
internal class FainCraftGame : FainGameEngine
{
    private SceneManager _sceneManager;

    public FainCraftGame() : base(1600, 900, windowTitle: "FainCraft")
    {
        _sceneManager = new SceneManager(new SceneFactory());
    }

    protected override void Load()
    {
        //_sceneManager.LoadScene(new StartMenuScene());
        _sceneManager.LoadScene(new SingleplayerWorld());
    }
}
