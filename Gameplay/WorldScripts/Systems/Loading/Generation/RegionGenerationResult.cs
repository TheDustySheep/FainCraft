using FainCraft.Gameplay.OldWorldScripts.Editing;
using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;
internal class RegionGenerationResult
{
    public RegionCoord RegionCoord;
    public RegionData RegionData;
    public RegionEditList VoxelEdits;

    public RegionGenerationResult(RegionCoord regionCoord, RegionData regionData, RegionEditList voxelEdits)
    {
        RegionCoord = regionCoord;
        RegionData = regionData;
        VoxelEdits = voxelEdits;
    }
}
