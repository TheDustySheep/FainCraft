using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Editing;
internal struct SetSingleVoxel : IVoxelEdit
{
    readonly bool immediate;
    readonly VoxelState newVoxel;

    public VoxelCoordGlobal GlobalCoord { get; set; }

    public SetSingleVoxel(VoxelCoordGlobal coord, VoxelState newVoxel, bool immediate = false)
    {
        GlobalCoord = coord;

        this.newVoxel = newVoxel;
        this.immediate = immediate;
    }

    public VoxelEditResult Execute(IVoxelEditable worldData)
    {
        VoxelState newVoxel = this.newVoxel;

        worldData.EditVoxelData
        (
            GlobalCoord,
            _oldVoxel => newVoxel,
            immediate
        );

        return new VoxelEditResult((VoxelCoordLocal)GlobalCoord);
    }
}
