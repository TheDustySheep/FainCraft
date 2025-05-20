using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld.Biomes.SurfaceDecoration;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld.Biomes.SurfacePainting;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld.Biomes.Types
{
    internal interface IBiome
    {
        public ISurfacePainter Painter { get; }
        public ISurfaceDecorator Decorator { get; }

        float SampleHeight(RegionMaps maps, VoxelCoord2DGlobal coord);
    }
}
