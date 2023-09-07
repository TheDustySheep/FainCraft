using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts.Chunking;
using FainEngine_v2.Core;

namespace FainCraft;
internal class FainCraftGameEngine : FainGameEngine
{
    public FainCraftGameEngine() : base(windowTitle: "FainCraft")
    {
    }

    protected override void Load()
    {
        //var cam = EntityManager.SpawnEntity<CameraEntity>();
        //ICamera.SetMainCamera(cam);
        //var testModel = EntityManager.SpawnEntity<TestModel>();
        //EntityManager.SpawnEntity<SystemDiagnostics>();
        var world = EntityManager.SpawnEntity<World>();
        EntityManager.SpawnEntity(new PlayerEntity(world.WorldData));
    }
}
