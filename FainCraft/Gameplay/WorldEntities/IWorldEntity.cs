using FainEngine_v2.Physics.AABB;

namespace FainCraft.Gameplay.WorldEntities;
public interface IWorldEntity
{
    public StaticAABB Bounds { get; }
}
