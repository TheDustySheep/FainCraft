using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts;
using FainCraft.Gameplay.WorldScripts.Systems;
using FainEngine_v2.Core;

namespace FainCraft;
internal class FainCraftGameEngine : FainGameEngine
{
    public FainCraftGameEngine() : base(windowTitle: "FainCraft")
    {
    }

    protected override void Load()
    {
        EntityManager.SpawnEntity<SystemDiagnostics>();
        var world = EntityManager.SpawnEntity<World>();
        var player = EntityManager.SpawnEntity(new PlayerEntity(world));
    }
}
