using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Voxels;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.SurfaceDecoration
{
    internal class TreeDecorator : ISurfaceDecorator
    {
        readonly Random random;

        readonly VoxelState AIR;
        readonly VoxelState GRASS;
        readonly VoxelState LOG;

        public TreeDecorator(VoxelIndexer indexer, int seed)
        {
            random = new Random(seed);

            AIR   = new() { Index = indexer.GetIndex("Air")   };
            GRASS = new() { Index = indexer.GetIndex("Grass") };
            LOG   = new() { Index = indexer.GetIndex("Log")   };
        }

        public void HandleSpawn(RegionData regionData, RegionCoord regionCoord)
        {
            for (int i = 0; i < 5; i++)
            {
                int l_x = random.Next(CHUNK_SIZE);
                int l_z = random.Next(CHUNK_SIZE);

                // Iterate downwards from sky
                for (int c_y = REGION_Y_TOTAL_COUNT - 1; c_y >= 0; c_y--)
                {
                    var chunk = regionData.Chunks[c_y];

                    for (int l_y = CHUNK_SIZE - 1; l_y >= 0; l_y--)
                    {
                        var voxelState = chunk[l_x, l_y, l_z];
                        if (voxelState == GRASS)
                        {
                            int h = random.Next(4);

                            // TODO this will throw an exception if the top most block is a grass block
                            for (int j = 1; j <= 3 + h; j++)
                            {
                                chunk[l_x, l_y + j, l_z] = LOG;
                            }
                        }
                    }
                }
            }
        }
    }
}
