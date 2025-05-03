using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Editing;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration;
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
