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
        public Task<RegionData?> LoadAsync(RegionCoord coord)
        {
            return Task.FromResult<RegionData?>(null);
        }

        public Task<bool> SaveAsync(RegionCoord coord, RegionData region)
        {
            return Task.FromResult(false);
        }
    }
}
