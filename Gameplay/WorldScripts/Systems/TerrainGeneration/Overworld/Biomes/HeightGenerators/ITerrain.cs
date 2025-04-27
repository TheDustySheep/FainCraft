using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.HeightGenerators
{
    internal interface ITerrain
    {
        float Generate(RegionMaps maps, VoxelCoord2DGlobal coord);
    }
}
