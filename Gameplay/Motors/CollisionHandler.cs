using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainEngine_v2.Core;
using FainEngine_v2.Extensions;
using FainEngine_v2.Physics.AABB;
using System.Numerics;

namespace FainCraft.Gameplay.Motors;
internal class CollisionHandler
{
    const int _physicsIterations = 1;

    VoxelIndexer indexer;
    IWorldData worldData;

    public CollisionHandler(IWorldData worldData)
    {
        this.worldData = worldData;
        indexer = worldData.Indexer;
    }

    public GroundedState UpdateGrounded(StaticAABB entity)
    {
        // Check below entity
        entity.Position.Y -= 0.002f;
        entity.Size.Y = 0.502f;

        GlobalVoxelCoord playerCoord = new GlobalVoxelCoord(entity.Position);
        GlobalVoxelCoord minVoxel = playerCoord + new GlobalVoxelCoord(-1, -1, -1);
        GlobalVoxelCoord maxVoxel = playerCoord + new GlobalVoxelCoord(1, 1, 1);

        float bestDotProd = 0f;
        Vector3 bestGroundAngle = Vector3.Zero;

        // Add walls to the nearby colliders to list
        for (var i_y = minVoxel.Y; i_y <= maxVoxel.Y; i_y++)
        {
            for (var i_z = minVoxel.Z; i_z <= maxVoxel.Z; i_z++)
            {
                for (var i_x = minVoxel.X; i_x <= maxVoxel.X; i_x++)
                {
                    GlobalVoxelCoord voxelCoord = new GlobalVoxelCoord(i_x, i_y, i_z);

                    worldData.GetVoxelData(voxelCoord, out var voxelData);

                    // Discard non-solid voxels
                    if (!indexer.GetVoxelType(voxelData.Index).Physics_Solid)
                        continue;

                    var voxel = new StaticAABB()
                    {
                        Position = (Vector3)voxelCoord,
                        Size = Vector3.One
                    };

                    if (CollisionResolver.IsOverlapping(entity, voxel))
                    {
                        var overlap = CollisionResolver.CalculateOverlap(entity, voxel);
                        var resolve = CollisionResolver.ResolveOverlap(entity, overlap);
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
        entity.Delta /= _physicsIterations;

        for (int i = 0; i < _physicsIterations; i++)
            PhysicsStep(ref entity, indexer);

        entity.Delta *= _physicsIterations;
    }

    List<StaticAABB> colliders = new();
    private void PhysicsStep(ref DynamicAABB playerAABB, VoxelIndexer indexer)
    {
        GlobalVoxelCoord playerCoord = new GlobalVoxelCoord(playerAABB.Position);
        GlobalVoxelCoord minVoxel = playerCoord + new GlobalVoxelCoord(-1, -1, -1);
        GlobalVoxelCoord maxVoxel = playerCoord + new GlobalVoxelCoord(1, 2, 1);

        colliders.Clear();

        // Add walls to the nearby colliders to list
        for (var i_y = minVoxel.Y; i_y <= maxVoxel.Y; i_y++)
        {
            for (var i_z = minVoxel.Z; i_z <= maxVoxel.Z; i_z++)
            {
                for (var i_x = minVoxel.X; i_x <= maxVoxel.X; i_x++)
                {
                    GlobalVoxelCoord voxelCoord = new GlobalVoxelCoord(i_x, i_y, i_z);

                    worldData.GetVoxelData(voxelCoord, out var voxelData);

                    // Discard non-solid voxels
                    if (!indexer.GetVoxelType(voxelData.Index).Physics_Solid)
                        continue;

                    colliders.Add(new StaticAABB()
                    {
                        Position = (Vector3)voxelCoord,
                        Size = Vector3.One
                    });
                }
            }
        }

        playerAABB = CollisionResolver.ResolveCollision(playerAABB, colliders, CollisionMode.Slide);

        playerAABB.Position += playerAABB.Delta;
    }
}
