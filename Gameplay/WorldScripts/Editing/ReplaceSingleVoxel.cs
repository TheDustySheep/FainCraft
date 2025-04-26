using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Editing;
internal struct ReplaceSingleVoxel : IVoxelEdit
{
    readonly bool immediate;
    readonly VoxelState oldVoxel;
    readonly VoxelState newVoxel;

    public VoxelCoordGlobal GlobalCoord { get; set; }

    public ReplaceSingleVoxel(VoxelCoordGlobal coord, VoxelState oldVoxel, VoxelState newVoxel, bool immediate = false)
    {
        GlobalCoord = coord;

        this.oldVoxel = oldVoxel;
        this.newVoxel = newVoxel;
        this.immediate = immediate;
    }

    public VoxelEditResult Execute(IVoxelEditable worldData)
    {
        VoxelState newVoxel = this.newVoxel;
        VoxelState oldVoxel = this.oldVoxel;

        worldData.EditVoxelData
        (
            GlobalCoord,
            currentVoxel =>
            {
                if (currentVoxel == oldVoxel)
                    return newVoxel;
                return currentVoxel;
            },
            immediate
        );

        return new VoxelEditResult((VoxelCoordLocal)GlobalCoord);
    }
}
