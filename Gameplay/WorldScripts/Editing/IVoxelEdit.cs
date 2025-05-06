using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Editing;
public interface IVoxelEdit
{
    public VoxelState Execute(VoxelState oldState);
}
