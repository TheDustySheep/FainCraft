using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionSerialization
{
    public interface IRegionSerializer
    {
        public void Serialize(FileStream stream, RegionCoord coord, RegionData data);
        public bool Deserialize(FileStream stream, out RegionCoord coord, out RegionData data);
    }
}
