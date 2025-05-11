using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Editing;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes;
using FainCraft.Gameplay.WorldScripts.Voxels;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld
{
    internal class OverworldGenerator : ITerrainGenerator
    {
        readonly BiomesDecider _biomeDecider;
        readonly BiomesFactory _biomeFactory;

        readonly RegionMaps _maps = new();

        public OverworldGenerator(VoxelIndexer indexer, int seed = 1337)
        {
            _biomeFactory = new BiomesFactory(indexer, seed);
            _biomeDecider = new BiomesDecider(_biomeFactory, seed);
        }

        public RegionGenerationResult Generate(RegionCoord regionCoord)
        {
            VoxelCoordGlobal regionGlobal = (VoxelCoordGlobal)regionCoord;

            var regionData = new RegionData();

            // Generate the surrounding biomes
            _biomeDecider.SampleBiomes(_maps, regionCoord);

            // Sample terrain surface
            BiomesSampler.SampleSurfaceHeights(_maps, regionCoord);

            // Iterate over the chunks
            HandlePainting(regionData, regionCoord);

            // Decorate surface
            var edits = HandleDecoration(regionData, regionCoord);

            return new RegionGenerationResult(regionCoord, regionData, edits);
        }

        private void HandlePainting(RegionData regionData, RegionCoord regionCoord)
        {
            for (int y = 0; y < REGION_TOTAL_CHUNKS; y++)
            {
                var chunkCoord = new ChunkCoord(regionCoord, y - REGION_NEG_CHUNKS);
                PaintChunk(regionData.Chunks[y], chunkCoord);
            }
        }

        private void PaintChunk(ChunkData data, ChunkCoord chunkCoord)
        {
            Span<VoxelState> column = stackalloc VoxelState[CHUNK_SIZE];
            for (int l_x = 0; l_x < CHUNK_SIZE; l_x++)
            {
                for (int l_z = 0; l_z < CHUNK_SIZE; l_z++)
                {
                    var biome = _maps.GetBiome(l_x, l_z);
                    var painter = biome.Painter;

                    painter.Paint(_maps, column, chunkCoord, l_x, l_z);

                    for (int l_y = 0; l_y < CHUNK_SIZE; l_y++)
                    {
                        // Copy vertical column over to the chunk data
                        VoxelCoordLocal localCoord = new(l_x, l_y, l_z);
                        data[localCoord.VoxelIndex] = column[l_y];
                    }
                }
            }
        }

        private RegionEditList HandleDecoration(RegionData regionData, RegionCoord regionCoord)
        {
            var edits = new RegionEditList();

            foreach (var biome in _maps.Biomes.Data.Distinct())
            {
                var decorator = biome.Decorator;
                decorator.HandleSpawn(edits, regionData, regionCoord);
            }

            edits.ApplyEdits(regionCoord, regionData);

            return edits;
        }
    }
}
