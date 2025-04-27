using FainCraft.Gameplay.WorldScripts.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Editing
{
    internal struct VoxelEditHolder
    {
        public RegionCoord RegionCoord;
        public VoxelCoordGlobal VoxelCoord;
        public IVoxelEdit Edit;
    }
}
