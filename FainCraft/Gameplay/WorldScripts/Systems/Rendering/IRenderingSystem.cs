using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering;

public interface IRenderingSystem
{
    public void Add(RegionCoord coord);
    public void Remove(RegionCoord coord);
}
