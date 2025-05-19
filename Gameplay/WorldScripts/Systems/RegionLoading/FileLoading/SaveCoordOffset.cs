using FainCraft.Gameplay.WorldScripts.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading
{
    public readonly struct SaveCoordOffset
    {
        public readonly ushort Index;

        public SaveCoordOffset(RegionCoord rCoord, uint yIndex)
        {
            uint x = SaveCoord.Convert_Region_Coord_To_Offset(rCoord.X);
            uint y = yIndex;
            uint z = SaveCoord.Convert_Region_Coord_To_Offset(rCoord.Z);

            Index = (ushort)(
                ((x & 15) << 0) |
                ((y & 15) << 4) |
                ((z & 15) << 8)
            );
        }

        public SaveCoordOffset(uint x, uint y, uint z)
        {
            Index = (ushort)(
                ((x & 15) << 0) |
                ((y & 15) << 4) |
                ((z & 15) << 8)
            );
        }

        public SaveCoordOffset(ushort index)
        {
            Index = index;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is SaveCoordOffset offset && Index == offset.Index;
        }
        public static bool operator ==(SaveCoordOffset left, SaveCoordOffset right)
        {
            return left.Index == right.Index;
        }

        public static bool operator !=(SaveCoordOffset left, SaveCoordOffset right)
        {
            return left.Index != right.Index;
        }

        public override readonly int GetHashCode()
        {
            return Index;
        }
    }
}
