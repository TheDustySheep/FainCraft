using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.HeightMaps;
using FainCraft.Gameplay.WorldScripts.Voxels;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.SurfaceDecoration
{
    internal class PainterOcean : ISurfacePainter
    {
        readonly VoxelState AIR;
        readonly VoxelState WATER;
        readonly VoxelState SAND;
        readonly VoxelState STONE;

        const int WATER_HEIGHT = 0;
        const int MAX_SAND_HEIGHT = 8;

        public PainterOcean(VoxelIndexer indexer)
        {
            AIR   = new() { Index = indexer.GetIndex("Air") };
            WATER = new() { Index = indexer.GetIndex("Water") };
            SAND  = new() { Index = indexer.GetIndex("Sand") };
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
                {
                    if (globalCoord.Y > WATER_HEIGHT)
                        voxel = AIR;
                    else
                        voxel = WATER;
                }
                else if (depth < 3 && globalCoord.Y < MAX_SAND_HEIGHT)
                    voxel = SAND;
                else
                    voxel = STONE;

                data[l_y] = voxel;
            }
        }
    }
}
