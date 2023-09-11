using FainEngine_v2.Physics.AABB;

namespace FainCraft.Gameplay.WorldEntities;
internal interface IWorldEntity
{
    public StaticAABB Bounds { get; }
}
