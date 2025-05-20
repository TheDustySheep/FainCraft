using FainCraft.Gameplay.WorldScripts.Data.Voxels;

namespace FainCraft.Gameplay.OldWorldScripts.Editing
{
    internal readonly struct VoxelEditSet : IVoxelEdit
    {
        readonly VoxelState _newState;

        public VoxelEditSet(VoxelState newState)
        {
            _newState = newState;
        }

        public readonly VoxelState Execute(VoxelState oldState)
        {
            return _newState;
        }
    }
}
