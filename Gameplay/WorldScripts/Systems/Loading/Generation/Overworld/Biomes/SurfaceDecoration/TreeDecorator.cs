using FainCraft.Gameplay.OldWorldScripts.Editing;
using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld.Biomes.SurfaceDecoration
{
    internal class TreeDecorator : ISurfaceDecorator
    {
        readonly Random random;

        readonly VoxelState AIR;
        readonly VoxelState GRASS;
        readonly VoxelState LOG;
        readonly VoxelState LEAVES;

        public TreeDecorator(IVoxelIndexer indexer, int seed)
        {
            random = new Random(seed);

            AIR    = new() { Index = indexer.GetIndex("Air") };
            GRASS  = new() { Index = indexer.GetIndex("Grass") };
            LOG    = new() { Index = indexer.GetIndex("Log") };
            LEAVES = new() { Index = indexer.GetIndex("Leaves") };
        }

        public void HandleSpawn(RegionEditList edits, RegionData regionData, RegionCoord regionCoord)
        {
            for (int i = 0; i < 5; i++)
            {
                int l_x = random.Next(CHUNK_SIZE);
                int l_z = random.Next(CHUNK_SIZE);

                var globalCoord = FindTreeSpawn(edits, regionData, regionCoord, l_x, l_z);

                if (globalCoord is null)
                    continue;

                PlaceTree(edits, globalCoord.Value);
            }
        }

        private VoxelCoordGlobal? FindTreeSpawn(RegionEditList edits, RegionData regionData, RegionCoord regionCoord, int l_x, int l_z)
        {
            // Iterate downwards from sky
            for (int r_y = REGION_TOTAL_CHUNKS - 1; r_y >= 0; r_y--)
            {
                var chunk = regionData.Chunks[r_y];
                int c_y = r_y - REGION_NEG_CHUNKS;

                for (int l_y = CHUNK_SIZE - 1; l_y >= 0; l_y--)
                {
                    var voxelState = chunk[l_x, l_y, l_z];
                    if (voxelState != GRASS)
                        continue;

                    var chunkCoord = new ChunkCoord(regionCoord, c_y);
                    var globalCoord = new VoxelCoordGlobal(chunkCoord, new VoxelCoordLocal(l_x, l_y, l_z));
                    globalCoord.Y += 1;

                    // Found suitable location
                    return globalCoord;
                }
            }

            return null;
        }

        private void PlaceTree(RegionEditList edits, VoxelCoordGlobal globalCoord)
        {
            int h = random.Next(3, 7);

            for (int y = 0; y < h; y++)
            {
                edits.AddEdit(
                    globalCoord.Offset(0, y, 0),
                    new VoxelEditSet(LOG)
                );
            }

            var leafRadius = h switch
            {
                3 => 1,
                4 => 2,
                5 => 2,
                6 => 3,
                _ => 1,
            };

            for (int x = -leafRadius; x <= leafRadius; x++)
            {
                for (int z = -leafRadius; z <= leafRadius; z++)
                {
                    for (int y = h - 2; y < (h - 1) * 2; y++)
                    {
                        edits.AddEdit(
                            globalCoord.Offset(x, y, z),
                            new VoxelEditReplaceTarget(LEAVES, AIR)
                        );
                    }
                }
            }
        }
    }
}
