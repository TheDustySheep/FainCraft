using FainCraft.Gameplay.WorldEntities;
using FainCraft.Gameplay.OldWorldScripts;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Physics;
using FainEngine_v2.Physics.AABB;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using Silk.NET.Maths;
using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainCraft.Gameplay.WorldScripts.Storage;

namespace FainCraft.Gameplay.PlayerScripts;

internal class WorldEditor
{
    private readonly Transform _camTransform;
    private readonly IVoxelIndexer _indexer;
    private readonly IVoxelDataStore _voxelDataStore;
    private readonly IWorldEntityController _entityController;

    public WorldEditor(Transform camTransform, IServiceProvider provider)
    {
        _camTransform = camTransform;
        _indexer = provider.Get<IVoxelIndexer>();
        _voxelDataStore = provider.Get<IVoxelDataStore>();
        _entityController = provider.Get<IWorldEntityController>();
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

                foreach (var entity in _entityController.Entities)
                {
                    if (AABBResolver.IsOverlapping(voxelAABB, entity.Bounds))
                        return;
                }

                EditVoxel(faceVox, new VoxelState() { Index = _indexer.GetIndex("Cobblestone_Slab") });
            }
        }
    }

    private void EditVoxel(Vector3D<int> coord, VoxelState newVoxel)
    {
        VoxelCoordGlobal voxCoord = new VoxelCoordGlobal(coord);
        _voxelDataStore.EditVoxelState(voxCoord, oldVoxel =>
        {
            return newVoxel;
        });
    }

    private bool RaycastVoxel(out VoxelHit hit)
    {
        return VoxelRaycaster.RaycastBlock
        (
            new Ray()
            {
                Direction = _camTransform.Forward,
                Origin = _camTransform.GlobalPosition
            },
            10f,
            voxPos =>
            {
                VoxelCoordGlobal coord = new VoxelCoordGlobal(voxPos);

                if (!_voxelDataStore.GetVoxelState(coord, out var voxelData))
                    return false;

                var type = _indexer.GetVoxelType(voxelData.Index);

                return type.Physics_Solid;
            },
            out hit
        );
    }
}
