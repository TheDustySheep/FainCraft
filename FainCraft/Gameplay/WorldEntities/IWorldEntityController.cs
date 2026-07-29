namespace FainCraft.Gameplay.WorldEntities;

public interface IWorldEntityController
{
    public HashSet<IWorldEntity> Entities { get; }
    public void RegisterEntity(IWorldEntity entity);
    public void UnregisterEntity(IWorldEntity entity);
}