using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal interface ITerrainGenerationSystem
{
    public void Tick();
    public void Request(RegionCoord coord);
}
