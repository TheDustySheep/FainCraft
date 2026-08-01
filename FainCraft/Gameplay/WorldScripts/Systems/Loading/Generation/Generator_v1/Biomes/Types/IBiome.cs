using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v1;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v1.Biomes.SurfaceDecoration;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v1.Biomes.SurfacePainting;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v1.Biomes.Types
{
    internal interface IBiome
    {
        public ISurfacePainter Painter { get; }
        public ISurfaceDecorator Decorator { get; }

        float SampleHeight(RegionMaps maps, VoxelCoord2DGlobal coord);
    }
}
