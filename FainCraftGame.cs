using FainCraft.Scenes;
using FainEngine_v2.Core;
using FainEngine_v2.UI.Drawing;
using FainEngine_v2.UI.FontRendering;
using FainEngine_v2.UI.Fonts;
using FainEngine_v2.Utils;

namespace FainCraft;
internal class FainCraftGame : FainGameEngine
{
    private SceneManager _sceneManager;

    public FainCraftGame() : base(1600, 900, windowTitle: "FainCraft")
    {
        RegisterUIDependencies();

        _sceneManager = DependencyInjector.RegisterSingleton(() => new SceneManager(new SceneFactory()));
    }

    protected override void Load()
    {
        _sceneManager.LoadScene(new StartMenuScene());
        //_sceneManager.LoadScene(new SingleplayerWorld());
    }

    private void RegisterUIDependencies()
    {
        var fontController = new FontController();
        var uiMatManager = new UIMaterialManager(fontController);

        DependencyInjector.RegisterSingleton<IFontController>(fontController);
        DependencyInjector.RegisterSingleton<UIMaterialManager>(uiMatManager);
    }
}
