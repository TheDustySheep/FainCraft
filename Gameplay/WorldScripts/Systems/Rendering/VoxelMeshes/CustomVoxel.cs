using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes
{
    public class CustomVoxel
    {
        public CustomVoxel(uint[] quadIndexes)
        {
            QuadIndexes = quadIndexes;
        }

        public uint[] QuadIndexes { get; private set; }
    }
}
