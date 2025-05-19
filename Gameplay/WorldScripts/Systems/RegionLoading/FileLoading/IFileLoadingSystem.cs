using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading
{
    public interface IFileLoadingSystem
    {
        public Task<RegionData?> LoadAsync(RegionCoord coord);
        public Task<bool> SaveAsync(RegionCoord coord, RegionData region);
    }
}
