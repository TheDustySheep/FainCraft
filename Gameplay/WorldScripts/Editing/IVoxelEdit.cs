using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Editing;
internal interface IVoxelEdit
{
    public VoxelState Execute(VoxelState oldState);
}
