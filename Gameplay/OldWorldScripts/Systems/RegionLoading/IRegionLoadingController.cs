using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.OldWorldScripts.Systems.RegionLoading;
internal interface IRegionLoadingController
{
    void Load(RegionCoord coord);
    void Unload(RegionCoord coord);
    void Tick();
}