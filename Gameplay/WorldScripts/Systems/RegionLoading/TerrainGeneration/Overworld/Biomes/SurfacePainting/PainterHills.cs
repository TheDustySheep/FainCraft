using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainEngine_v2.Utils;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.SurfacePainting
{
    internal class PainterHills : ISurfacePainter
    {
        readonly VoxelState AIR;
        readonly VoxelState STONE;
        readonly VoxelState SNOW;
        readonly VoxelState GRASS;
        readonly VoxelState DIRT;

        FastNoiseLite noise;

        readonly int _snowLine;
        readonly int _grassLine = 40;

        public PainterHills(VoxelIndexer indexer, int snowLine)
        {
            _snowLine = snowLine;

            AIR   = new() { Index = indexer.GetIndex("Air")   };
            STONE = new() { Index = indexer.GetIndex("Stone") };
            SNOW  = new() { Index = indexer.GetIndex("Snow")  };
            GRASS = new() { Index = indexer.GetIndex("Grass") };
            DIRT  = new() { Index = indexer.GetIndex("Dirt")  };

            noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Value);
            noise.SetFrequency(1f);
        }

        public void Paint(RegionMaps maps, Span<VoxelState> data, ChunkCoord chunkCoord, int l_x, int l_z)
        {
            int surfaceHeight = (int)maps.SurfaceHeight[l_x, l_z];


            VoxelCoordLocal c_l_coord = new(l_x, 0, l_z);
            VoxelCoordGlobal c_g_coord = new(chunkCoord, c_l_coord);
            float variation = noise.GetNoise(c_g_coord.X, c_g_coord.Z) * 5f;

            for (int l_y = 0; l_y < CHUNK_SIZE; l_y++)
            {
                VoxelCoordLocal localCoord = new(l_x, l_y, l_z);
                VoxelCoordGlobal globalCoord = new(chunkCoord, localCoord);

                VoxelState voxel;

                int depth = surfaceHeight - globalCoord.Y;

                if (depth < 0)
                    voxel = AIR;
                else if (depth == 0)
                {
                    if (globalCoord.Y > _snowLine + variation)
                        voxel = SNOW;
                    else if (globalCoord.Y < _grassLine + variation)
                        voxel = GRASS;
                    else
                        voxel = STONE;
                }
                else if (depth <= 3 && globalCoord.Y < _grassLine + variation)
                {
                    voxel = DIRT;
                }
                else
                {
                    voxel = STONE;
                }

                data[l_y] = voxel;
            }
        }
    }
}
