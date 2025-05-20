using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld.Biomes.SurfacePainting
{
    internal class PainterPlains : ISurfacePainter
    {
        readonly VoxelState AIR;
        readonly VoxelState GRASS;
        readonly VoxelState DIRT;
        readonly VoxelState STONE;

        public PainterPlains(IVoxelIndexer indexer)
        {
            AIR   = new() { Index = indexer.GetIndex("Air") };
            GRASS = new() { Index = indexer.GetIndex("Grass") };
            DIRT  = new() { Index = indexer.GetIndex("Dirt") };
            STONE = new() { Index = indexer.GetIndex("Stone") };
        }

        public void Paint(RegionMaps maps, Span<VoxelState> data, ChunkCoord chunkCoord, int l_x, int l_z)
        {
            int surfaceHeight = (int)maps.SurfaceHeight[l_x, l_z];

            for (int l_y = 0; l_y < CHUNK_SIZE; l_y++)
            {
                VoxelCoordLocal localCoord = new(l_x, l_y, l_z);
                VoxelCoordGlobal globalCoord = new(chunkCoord, localCoord);

                VoxelState voxel;

                int depth = surfaceHeight - globalCoord.Y;

                if (depth < 0)
                    voxel = AIR;
                else if (depth == 0)
                    voxel = GRASS;
                else if (depth < 3)
                    voxel = DIRT;
                else
                    voxel = STONE;

                data[l_y] = voxel;
            }
        }
    }
}
