using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.Types;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes
{
    internal static class BiomesSampler
    {
        static readonly float[,] KERNAL;

        // Generate the kernal
        static BiomesSampler()
        {
            int radius = RegionMaps.OVERSAMPLE_RADIUS;
            float sigma = 1.0f;

            int size = radius * 2 + 1;
            float[,] kernel = new float[size, size];

            float sum = 0f;
            float twoSigmaSq = 2 * sigma * sigma;

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    float distanceSq = x * x + y * y;
                    float value = (float)Math.Exp(-distanceSq / twoSigmaSq);
                    kernel[x + radius, y + radius] = value;
                    sum += value;
                }
            }

            // Normalize kernel so all weights sum to 1
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    kernel[x, y] /= sum;
                }
            }

            KERNAL = kernel;
        }

        public static void SampleSurfaceHeights(RegionMaps maps, RegionCoord regionCoord)
        {
            const int SAMPLE_RADIUS = RegionMaps.OVERSAMPLE_RADIUS;
            const int SAMPLE_AREA = RegionMaps.OVERSAMPLE_AREA;

            Dictionary<IBiome, float> weightedBiomes = new(SAMPLE_AREA);

            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    weightedBiomes.Clear();
                    for (int dx = -SAMPLE_RADIUS; dx <= SAMPLE_RADIUS; dx++)
                    {
                        for (int dz = -SAMPLE_RADIUS; dz <= SAMPLE_RADIUS; dz++)
                        {
                            var biome = maps.GetBiomeOversampled(
                                x + dx + SAMPLE_RADIUS,
                                z + dz + SAMPLE_RADIUS);

                            if (weightedBiomes.ContainsKey(biome))
                                weightedBiomes[biome] += KERNAL[dx + SAMPLE_RADIUS, dz + SAMPLE_RADIUS];
                            else
                                weightedBiomes[biome] = KERNAL[dx + SAMPLE_RADIUS, dz + SAMPLE_RADIUS];
                        }
                    }

                    float sum = 0f;
                    foreach (var kvp in weightedBiomes)
                    {
                        var globalCoord2D = new VoxelCoord2DGlobal(regionCoord, x, z);

                        sum += kvp.Value * kvp.Key.SampleHeight(maps, globalCoord2D);
                    }

                    maps.SurfaceHeight[x, z] = sum;
                }
            }
        }
    }
}
