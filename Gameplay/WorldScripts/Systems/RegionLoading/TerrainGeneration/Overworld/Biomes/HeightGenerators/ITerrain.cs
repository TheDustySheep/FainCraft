using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.HeightGenerators
{
    internal interface ITerrain
    {
        float Generate(RegionMaps maps, VoxelCoord2DGlobal coord);
    }
}
