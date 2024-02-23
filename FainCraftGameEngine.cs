using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts;
using FainEngine_v2.Core;
using FainEngine_v2.UI;

namespace FainCraft;
internal class FainCraftGameEngine : FainGameEngine
{
    public FainCraftGameEngine() : base(1600*2, 900*2, windowTitle: "FainCraft")
    {
    }

    protected override void Load()
    {
        //EntityManager.SpawnEntity<SystemDiagnostics>();
        var ui = EntityManager.SpawnEntity<UICanvas>();
        var world = EntityManager.SpawnEntity<World>();
        var player = EntityManager.SpawnEntity(new PlayerEntity(world));
    }
}
