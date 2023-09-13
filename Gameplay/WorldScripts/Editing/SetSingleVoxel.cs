using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Editing;
internal struct SetSingleVoxel : IVoxelEdit
{
    readonly bool immediate;
    readonly VoxelData newVoxel;

    public GlobalVoxelCoord GlobalCoord { get; set; }

    public SetSingleVoxel(GlobalVoxelCoord coord, VoxelData newVoxel, bool immediate = false)
    {
        GlobalCoord = coord;

        this.newVoxel = newVoxel;
        this.immediate = immediate;
    }

    public VoxelEditResult Execute(IVoxelEditable worldData)
    {
        VoxelData newVoxel = this.newVoxel;

        worldData.EditVoxelData
        (
            GlobalCoord,
            _oldVoxel => newVoxel,
            immediate
        );

        return new VoxelEditResult((LocalVoxelCoord)GlobalCoord);
    }
}
