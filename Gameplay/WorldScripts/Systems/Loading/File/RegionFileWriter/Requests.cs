using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Files;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.RegionFileWriter;

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