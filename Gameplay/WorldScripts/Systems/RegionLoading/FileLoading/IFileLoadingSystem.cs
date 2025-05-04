using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading
{
    public interface IFileLoadingSystem
    {
        public bool Request(RegionCoord coord);
        public IEnumerable<(RegionCoord, RegionData)> GetComplete();
        void Save(RegionCoord coord, RegionData region);
    }
}
