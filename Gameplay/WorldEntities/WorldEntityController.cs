namespace FainCraft.Gameplay.WorldEntities;
internal class WorldEntityController : IWorldEntityController
{
    public HashSet<IWorldEntity> Entities { get; private set; } = new();

    public void RegisterEntity(IWorldEntity entity)
    {
        Entities.Add(entity);
    }

    public void UnregisterEntity(IWorldEntity entity)
    {
        Entities.Remove(entity);
    }
}
