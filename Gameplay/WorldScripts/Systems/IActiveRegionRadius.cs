using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.WorldScripts.Systems;

public interface IActiveRegionRadius
{
    public event Action<RegionCoord>? Load;
    public event Action<RegionCoord>? Unload;

    public void Calculate();
}