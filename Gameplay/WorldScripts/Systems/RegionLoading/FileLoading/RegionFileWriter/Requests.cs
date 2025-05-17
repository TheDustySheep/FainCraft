using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.RegionFileWriter;

public struct LoadRequest
{
    public required SaveCoord SaveCoord;
    public required Stream Stream;
    public required RegionCoord[] RegionCoords;
}

public struct LoadResult
{
    public required Dictionary<RegionCoord, RegionData?> Regions;
}

public struct SaveRequest
{
    public required SaveCoord SaveCoord;
    public required Stream Stream;
    public required Dictionary<RegionCoord, RegionData> Regions;
}

public struct SaveResult
{
    public required Dictionary<RegionCoord, bool> Regions;
}