using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.SurfacePainting
{
    internal interface ISurfacePainter
    {
        public void Paint(RegionMaps maps, Span<VoxelState> data, ChunkCoord chunkCoord, int l_x, int l_z);
    }
}
