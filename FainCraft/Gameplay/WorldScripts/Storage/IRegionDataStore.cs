using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Storage
{
    public interface IRegionDataStore
    {
        public bool GetRegion(RegionCoord coord, [NotNullWhen(true)] out RegionData? data);
        public bool SetRegion(RegionCoord coord, RegionData? data);
        public bool EditRegion(RegionCoord coord, Func<RegionData, bool> func);
    }
}
