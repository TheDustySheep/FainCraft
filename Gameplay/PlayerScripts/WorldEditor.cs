using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Physics;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;

internal class WorldEditor
{
    VoxelIndexer indexer;
    IWorldData worldData;
    Transform camTransform;

    public WorldEditor(Transform camTransform, IWorldData worldData)
    {
        this.camTransform = camTransform;
        this.worldData = worldData;
        indexer = worldData.Indexer;
    }

    public void Update()
    {
        if (RaycastVoxel(out var hit))
        {
            if (GameInputs.IsMouseDown(MouseButton.Left))
            {
                EditVoxel(hit, new VoxelData() { Index = 0 });
            }
            else if (GameInputs.IsMouseDown(MouseButton.Right))
            {
                EditVoxel(hit, new VoxelData() { Index = 5 });
            }
        }
    }

    private void EditVoxel(VoxelHit hit, VoxelData newVoxel)
    {
        GlobalVoxelCoord coord = new GlobalVoxelCoord(hit.VoxelPosition);
        worldData.EditVoxelData(coord, oldVoxel =>
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
                Direction = camTransform.Forward,
                Origin = camTransform.GlobalPosition
            },
            10f,
            voxPos =>
            {
                GlobalVoxelCoord coord = new GlobalVoxelCoord(voxPos);

                if (!worldData.GetVoxelData(coord, out var voxelData))
                    return false;

                var type = indexer.GetVoxelType(voxelData.Index);

                return type.Physics_Solid;
            },
            out hit
        );
    }
}
