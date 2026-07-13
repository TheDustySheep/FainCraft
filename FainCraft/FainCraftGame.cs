using FainCraft.Scenes;
using FainEngine_v2.Core;
using FainEngine_v2.Utils;
using Silk.NET.Assimp;

namespace FainCraft;
internal class FainCraftGame : FainGameEngine
{
    private SceneManager _sceneManager;

    public FainCraftGame() : base(1600, 900, windowTitle: "FainCraft")
    {
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
        _sceneManager.LoadScene(new StartMenuScene());
    }
}
