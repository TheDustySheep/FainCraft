using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Editing;
internal interface IVoxelEditable
{
    // Voxel
    public bool VoxelExists(GlobalVoxelCoord coord);
    public bool GetVoxelData(GlobalVoxelCoord coord, out VoxelData voxelData);
    public bool SetVoxelData(GlobalVoxelCoord coord, VoxelData newVoxelData, bool immediate = false);
    public bool EditVoxelData(GlobalVoxelCoord coord, Func<VoxelData, VoxelData> editFunc, bool immediate = false);
}
