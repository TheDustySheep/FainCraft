using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainCraft.Gameplay.WorldScripts.Storage;
using FainEngine_v2.Extensions;
using FainEngine_v2.Physics.AABB;
using System.Numerics;

namespace FainCraft.Gameplay.Motors;
public class CollisionHandler
{
    public const int IterationCount = 10;

    private readonly IVoxelIndexer _indexer;
    private readonly IVoxelDataStore _voxelData;

    public CollisionHandler(IServiceProvider serviceProvider)
    {
        _indexer   = serviceProvider.Get<IVoxelIndexer>();
        _voxelData = serviceProvider.Get<IVoxelDataStore>();
    }

    public GroundedState UpdateGrounded(StaticAABB entity)
    {
        // Check below entity
        entity.Position.Y -= 0.002f;
        entity.Size.Y = 0.502f;

        VoxelCoordGlobal playerCoord = new VoxelCoordGlobal(entity.Position);
        VoxelCoordGlobal minVoxel = playerCoord + new VoxelCoordGlobal(-1, -1, -1);
        VoxelCoordGlobal maxVoxel = playerCoord + new VoxelCoordGlobal(1, 1, 1);

        float bestDotProd = 0f;
        Vector3 bestGroundAngle = Vector3.Zero;

        // Add walls to the nearby colliders to _coords
        for (var i_y = minVoxel.Y; i_y <= maxVoxel.Y; i_y++)
        {
            for (var i_z = minVoxel.Z; i_z <= maxVoxel.Z; i_z++)
            {
                for (var i_x = minVoxel.X; i_x <= maxVoxel.X; i_x++)
                {
                    VoxelCoordGlobal voxelCoord = new VoxelCoordGlobal(i_x, i_y, i_z);

                    _voxelData.GetVoxelState(voxelCoord, out var voxelState);

                    // Discard non-solid voxels
                    if (!_indexer.GetVoxelType(voxelState.Index).Physics_Solid)
                        continue;

                    var voxel = new StaticAABB()
                    {
                        Position = (Vector3)voxelCoord,
                        Size = Vector3.One
                    };

                    if (AABBResolver.IsOverlapping(entity, voxel))
                    {
                        var overlap = AABBResolver.CalculateOverlap(entity, voxel);
                        var resolve = AABBResolver.ResolveOverlap(entity, overlap);
                        var normal = resolve.Normalized();
                        var dotProd = Vector3.Dot(normal, Vector3.UnitY);

                        // Within 60 degrees
                        if (dotProd > bestDotProd)
                        {
                            bestDotProd = dotProd;
                            bestGroundAngle = normal;

                            if (bestDotProd == 1f)
                            {
                                return new GroundedState()
                                {
                                    IsGrounded = true,
                                    GroundNormal = bestGroundAngle,
                                };
                            }
                        }
                    }
                }
            }
        }

        // 60 degrees cutoff for grounded
        return new GroundedState()
        {
            IsGrounded = bestDotProd > 0.5f,
            GroundNormal = bestGroundAngle,
        };
    }

    public void VoxelCollisionsDynamic(ref DynamicAABB entity)
    {
        entity.Delta /= IterationCount;

        for (int i = 0; i < IterationCount; i++)
            PhysicsStep(ref entity);

        entity.Delta *= IterationCount;
    }

    readonly List<StaticAABB> colliders = new();
    private void PhysicsStep(ref DynamicAABB playerAABB)
    {
        VoxelCoordGlobal playerCoord = new VoxelCoordGlobal(playerAABB.Position);
        VoxelCoordGlobal minVoxel = playerCoord + new VoxelCoordGlobal(-1, -1, -1);
        VoxelCoordGlobal maxVoxel = playerCoord + new VoxelCoordGlobal( 1,  2,  1);

        colliders.Clear();

        // Add walls to the nearby colliders to _coords
        for (var i_y = minVoxel.Y; i_y <= maxVoxel.Y; i_y++)
        {
            for (var i_z = minVoxel.Z; i_z <= maxVoxel.Z; i_z++)
            {
                for (var i_x = minVoxel.X; i_x <= maxVoxel.X; i_x++)
                {
                    VoxelCoordGlobal voxelCoord = new VoxelCoordGlobal(i_x, i_y, i_z);

                    _voxelData.GetVoxelState(voxelCoord, out var voxelState);

                    // Discard non-solid voxels
                    if (!_indexer.GetVoxelType(voxelState.Index).Physics_Solid)
                        continue;

                    colliders.Add(new StaticAABB()
                    {
                        Position = (Vector3)voxelCoord,
                        Size = Vector3.One
                    });
                }
            }
        }

        playerAABB = AABBResolver.ResolveCollision(playerAABB, colliders, CollisionMode.Slide);

        playerAABB.Position += playerAABB.Delta;
    }

    public bool IsOverlapping(IAABB playerAABB, Func<VoxelType, bool> func)
    {
        VoxelCoordGlobal playerCoord = new VoxelCoordGlobal(playerAABB.Position);
        VoxelCoordGlobal minVoxel    = playerCoord + new VoxelCoordGlobal(-1, -1, -1);
        VoxelCoordGlobal maxVoxel    = playerCoord + new VoxelCoordGlobal( 1,  2,  1);

        // Add walls to the nearby colliders to _coords
        for (var i_y = minVoxel.Y; i_y <= maxVoxel.Y; i_y++)
        {
            for (var i_z = minVoxel.Z; i_z <= maxVoxel.Z; i_z++)
            {
                for (var i_x = minVoxel.X; i_x <= maxVoxel.X; i_x++)
                {
                    VoxelCoordGlobal voxelCoord = new VoxelCoordGlobal(i_x, i_y, i_z);

                    _voxelData.GetVoxelState(voxelCoord, out var voxelState);

                    var type = _indexer.GetVoxelType(voxelState.Index);

                    // Discard non-solid voxels
                    if (!func.Invoke(type))
                        continue;

                    bool result = AABBResolver.IsOverlapping(
                        playerAABB, 
                        new StaticAABB()
                        {
                            Position = (Vector3)voxelCoord,
                            Size = Vector3.One
                        }
                    );

                    if (result)
                        return true;
                }
            }
        }

        return false;
    }
}
