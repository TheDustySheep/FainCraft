namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.RegionFileWriter
{
    public interface IRegionSerializer
    {
        public SaveResult Save(SaveRequest request);
        public LoadResult Load(LoadRequest request);
    }
}
