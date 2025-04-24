using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Editing;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld_Old;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal class RegionGenerationResult
{
    public RegionData RegionData;
    public List<IVoxelEdit> VoxelEdits;

    public RegionGenerationResult(GenerationData data)
    {
        RegionData = data.regionData;
        VoxelEdits = data.outstandingEdits;
    }

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
