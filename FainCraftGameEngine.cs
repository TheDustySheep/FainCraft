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
        var world = EntityManager.SpawnEntity<World>();
        var player = EntityManager.SpawnEntity(new PlayerEntity(world.WorldData));
    }
}
