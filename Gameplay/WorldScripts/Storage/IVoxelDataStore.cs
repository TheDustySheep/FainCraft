using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;

namespace FainCraft.Gameplay.WorldScripts.Storage;

public interface IVoxelDataStore
{
    public bool GetVoxelState(VoxelCoordGlobal coord, out VoxelState state);
    public bool SetVoxelState(VoxelCoordGlobal coord, VoxelState state);
    public bool EditVoxelState(VoxelCoordGlobal coord, Func<VoxelState, VoxelState> editFunc);

    public Task<VoxelState?> GetVoxelStateAsync(VoxelCoordGlobal coord, CancellationToken token);
}
