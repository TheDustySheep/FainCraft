using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Editing
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
