using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v1;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v1.Biomes.HeightGenerators
{
    internal interface ITerrain
    {
        float Generate(RegionMaps maps, VoxelCoord2DGlobal coord);
    }
}
