using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading;
internal interface ILoadingController
{
    void OnLoad(RegionCoord coord);
    void OnUnload(RegionCoord coord);
    void Tick();
}