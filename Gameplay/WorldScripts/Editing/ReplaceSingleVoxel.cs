using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Editing;
internal struct ReplaceSingleVoxel : IVoxelEdit
{
    readonly bool immediate;
    readonly VoxelData oldVoxel;
    readonly VoxelData newVoxel;

    public GlobalVoxelCoord GlobalCoord { get; set; }

    public ReplaceSingleVoxel(GlobalVoxelCoord coord, VoxelData oldVoxel, VoxelData newVoxel, bool immediate = false)
    {
        GlobalCoord = coord;

        this.oldVoxel = oldVoxel;
        this.newVoxel = newVoxel;
        this.immediate = immediate;
    }

    public VoxelEditResult Execute(IVoxelEditable worldData)
    {
        VoxelData newVoxel = this.newVoxel;
        VoxelData oldVoxel = this.oldVoxel;

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

        return new VoxelEditResult((LocalVoxelCoord)GlobalCoord);
    }
}
