using FainCraft.Scenes;
using FainEngine_v2.Core;
using FainEngine_v2.Utils;

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

        if (gameInputs.IsKeyDown(Silk.NET.Input.Key.N))
            NextScene();

        _sceneManager.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        _sceneManager.FixedUpdate();
    }

    int sceneIdx = -1;
    private void NextScene()
    {
        sceneIdx++;
        if (sceneIdx > 2)
            sceneIdx = 0;

        IScene scene = sceneIdx switch
        {
            0 => new SingleplayerWorld(),
            1 => new UITestScene(),
            _ => new EmptyScene(),
        };
        _sceneManager.LoadScene(scene);
    }

    protected override void Load()
    {
        NextScene();
    }
}
