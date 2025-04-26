using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Editing;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal class RegionGenerationResult
{
    public RegionData RegionData;
    public List<IVoxelEdit> VoxelEdits;

    public RegionGenerationResult(RegionData data)
    {
        RegionData = data;
        VoxelEdits = new();
    }

    public RegionGenerationResult(RegionData regionData, List<IVoxelEdit> voxelEdits)
    {
        RegionData = regionData;
        VoxelEdits = voxelEdits;
    }
}
