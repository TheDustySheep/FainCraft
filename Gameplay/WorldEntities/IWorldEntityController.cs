namespace FainCraft.Gameplay.WorldEntities;

internal interface IWorldEntityController
{
    HashSet<IWorldEntity> Entities { get; }

    void RegisterEntity(IWorldEntity entity);
    void UnregisterEntity(IWorldEntity entity);
}