using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.SurfaceDecoration
{
    internal interface ISurfacePainter
    {
        public void Paint(RegionMaps maps, Span<VoxelState> data, ChunkCoord chunkCoord, int l_x, int l_z);
    }
}
