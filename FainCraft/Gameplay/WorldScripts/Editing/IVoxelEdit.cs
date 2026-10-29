using FainCraft.Gameplay.WorldScripts.Data.Voxels;

namespace FainCraft.Gameplay.OldWorldScripts.Editing;
public interface IVoxelEdit
{
    public VoxelState Execute(VoxelState oldState);
}
