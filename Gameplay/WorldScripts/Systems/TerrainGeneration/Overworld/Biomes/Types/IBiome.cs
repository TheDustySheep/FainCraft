using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.SurfaceDecoration;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.Types
{
    internal interface IBiome
    {
        public ISurfacePainter Painter { get; }
        public ISurfaceDecorator Decorator { get; }

        float SampleHeight(RegionMaps maps, VoxelCoord2DGlobal coord);
    }
}
