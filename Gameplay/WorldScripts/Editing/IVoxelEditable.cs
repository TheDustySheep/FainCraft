using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Editing;
internal interface IVoxelEditable
{
    // Voxel
    public bool VoxelExists(VoxelCoordGlobal coord);
    public bool GetVoxelData(VoxelCoordGlobal coord, out VoxelState voxelData);
    public bool SetVoxelData(VoxelCoordGlobal coord, VoxelState newVoxelData, bool immediate = false);
    public bool EditVoxelData(VoxelCoordGlobal coord, Func<VoxelState, VoxelState> editFunc, bool immediate = false);
}
