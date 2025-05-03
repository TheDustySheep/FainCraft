using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.SurfaceDecoration;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.SurfacePainting;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.Types
{
    internal interface IBiome
    {
        public ISurfacePainter Painter { get; }
        public ISurfaceDecorator Decorator { get; }

        float SampleHeight(RegionMaps maps, VoxelCoord2DGlobal coord);
    }
}
