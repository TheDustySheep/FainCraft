using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionFileWriter;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionSerialization
{
    public interface IRegionSerializer
    {
        public SaveResult Save(SaveRequest request);
        public LoadResult Load(LoadRequest request);
    }
}
