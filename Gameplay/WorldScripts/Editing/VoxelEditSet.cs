using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
