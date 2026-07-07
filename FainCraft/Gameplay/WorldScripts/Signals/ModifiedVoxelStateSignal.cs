using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;

namespace FainCraft.Gameplay.WorldScripts.Signals;

public struct ModifiedVoxelStateSignal
{
    public VoxelCoordGlobal Coord;
    public VoxelState OldState;
    public VoxelState NewState;
}
