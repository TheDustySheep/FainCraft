using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files
{
    public interface IFileLoadingSystem
    {
        public Task<RegionData?> LoadAsync(RegionCoord coord);
        public Task<bool> SaveAsync(RegionCoord coord, RegionData region);
    }
}
