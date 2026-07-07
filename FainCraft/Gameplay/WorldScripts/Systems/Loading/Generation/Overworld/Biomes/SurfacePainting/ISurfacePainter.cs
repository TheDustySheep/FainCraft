using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld.Biomes.SurfacePainting
{
    internal interface ISurfacePainter
    {
        public void Paint(RegionMaps maps, Span<VoxelState> data, ChunkCoord chunkCoord, int l_x, int l_z);
    }
}
