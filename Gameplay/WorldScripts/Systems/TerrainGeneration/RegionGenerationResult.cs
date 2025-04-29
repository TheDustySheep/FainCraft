using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Editing;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal class RegionGenerationResult
{
    public RegionData RegionData;
    public RegionEditList VoxelEdits;

    public RegionGenerationResult(RegionData regionData, RegionEditList voxelEdits)
    {
        RegionData = regionData;
        VoxelEdits = voxelEdits;
    }
}
