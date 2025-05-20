using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;

namespace FainCraft.Gameplay.WorldScripts.Signals;

public struct ModifiedRegionDataSignal
{
    public RegionCoord Coord;
    public RegionData Data;
}
