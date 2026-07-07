using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld.Biomes.HeightGenerators
{
    internal interface ITerrain
    {
        float Generate(RegionMaps maps, VoxelCoord2DGlobal coord);
    }
}
