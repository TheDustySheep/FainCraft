using FainCraft.Gameplay.WorldEntities;
using FainCraft.Gameplay.WorldScripts;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Physics;
using FainEngine_v2.Physics.AABB;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace FainCraft.Gameplay.PlayerScripts;

internal class WorldEditor
{
    readonly VoxelIndexer indexer;
    readonly IWorldData worldData;
    readonly Transform camTransform;
    readonly IWorldEntityController entityController;

    public WorldEditor(Transform camTransform, World world)
    {
        this.worldData = world.WorldData;
        this.entityController = world.WorldEntityController;
        this.camTransform = camTransform;
        indexer = worldData.Indexer;
    }

    public void Update()
    {
        if (RaycastVoxel(out var hit))
        {
            Gizmos.DrawVoxel(hit.VoxelPosition, System.Drawing.Color.Red);

            if (GameInputs.IsMouseDown(MouseButton.Left))
            {
                EditVoxel(hit.VoxelPosition, new VoxelState() { Index = 0 });
            }
            else if (GameInputs.IsMouseDown(MouseButton.Right))
            {
                var faceVox = hit.VoxelPosition + hit.VoxelNormal;

                var voxelAABB = new StaticAABB(faceVox);

                foreach (var entity in entityController.Entities)
                {
                    if (AABBResolver.IsOverlapping(voxelAABB, entity.Bounds))
                        return;
                }

                EditVoxel(faceVox, new VoxelState() { Index = 5 });
            }
        }
    }

    private void EditVoxel(Vector3D<int> coord, VoxelState newVoxel)
    {
        VoxelCoordGlobal voxCoord = new VoxelCoordGlobal(coord);
        worldData.EditVoxelData(voxCoord, oldVoxel =>
        {
            return newVoxel;
        }, true);
    }

    private bool RaycastVoxel(out VoxelHit hit)
    {
        return VoxelRaycaster.RaycastBlock
        (
            new Ray()
            {
                Direction = camTransform.Forward,
                Origin = camTransform.GlobalPosition
            },
            10f,
            voxPos =>
            {
                VoxelCoordGlobal coord = new VoxelCoordGlobal(voxPos);

                if (!worldData.GetVoxelData(coord, out var voxelData))
                    return false;

                var type = indexer.GetVoxelType(voxelData.Index);

                return type.Physics_Solid;
            },
            out hit
        );
    }
}
