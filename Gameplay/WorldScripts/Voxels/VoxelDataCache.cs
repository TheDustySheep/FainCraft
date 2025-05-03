using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Voxels
{
    public class VoxelDataCache<T>
    {
        public readonly T[] Data;

        public VoxelDataCache(VoxelType[] types, Func<VoxelType, T> func)
        {
            Data = new T[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                Data[i] = func.Invoke(types[i]);
            }
        }
    }
}
