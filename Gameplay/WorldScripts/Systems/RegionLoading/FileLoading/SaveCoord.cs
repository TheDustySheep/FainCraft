using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading
{
    public struct SaveCoord
    {
        public const int REGION_SIZE_POWER = 3;
        public const int REGION_SIZE = 1 << REGION_SIZE_POWER;
        public const int REGION_AREA = REGION_SIZE * REGION_SIZE;

        public int X;
        public int Z;

        public static explicit operator SaveCoord(RegionCoord rCoord)
        {
            return new SaveCoord
            {
                X = Convert_Region_Coord_To_Save_Coord(rCoord.X),
                Z = Convert_Region_Coord_To_Save_Coord(rCoord.Z),
            };
        }

        public static int Convert_Region_Coord_To_Save_Coord(int coord) => coord >> REGION_SIZE_POWER;
        public static int Convert_Region_Coord_To_Offset(int coord) => coord.Mod(REGION_SIZE);
    }
}
