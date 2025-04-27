using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Editing
{
    internal readonly struct VoxelEditReplaceTarget : IVoxelEdit
    {
        readonly VoxelState _newState;
        readonly VoxelState _targetState;

        public VoxelEditReplaceTarget(VoxelState newState, VoxelState targetState)
        {
            _newState = newState;
            _targetState = targetState;
        }

        public readonly VoxelState Execute(VoxelState oldState)
        {
            if (oldState == _targetState)
                return _newState;
            return oldState;
        }
    }
}
