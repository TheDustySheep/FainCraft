using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading;
internal interface IRegionLoadingController
{
    void Load(RegionCoord coord);
    void Unload(RegionCoord coord);
    void Tick();
}