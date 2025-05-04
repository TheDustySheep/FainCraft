using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading
{
    public class NullFileLoader : IFileLoadingSystem
    {
        public IEnumerable<(RegionCoord, RegionData)> GetComplete() { yield break; }
        public bool Request(RegionCoord coord) => false;
        public void Save(RegionCoord coord, RegionData region) { }
    }
}
