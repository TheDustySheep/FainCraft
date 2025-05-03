using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.Types;
using FainEngine_v2.Utils;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld
{
    internal class RegionMaps
    {
        public const int OVERSAMPLE_RADIUS = 3;
        public const int OVERSAMPLE_AREA = (OVERSAMPLE_RADIUS * 2 + 1) * (OVERSAMPLE_RADIUS * 2 + 1);

        public Array2D<IBiome> Biomes = new(CHUNK_SIZE + OVERSAMPLE_RADIUS * 2, CHUNK_SIZE + OVERSAMPLE_RADIUS * 2);

        public Array2D<float> Humidity = new(CHUNK_SIZE, CHUNK_SIZE);
        public Array2D<float> Mountains = new(CHUNK_SIZE, CHUNK_SIZE);
        public Array2D<float> Continents = new(CHUNK_SIZE, CHUNK_SIZE);
        public Array2D<float> Temperature = new(CHUNK_SIZE, CHUNK_SIZE);

        public Array2D<float> SurfaceHeight = new(CHUNK_SIZE, CHUNK_SIZE);

        public IBiome GetBiome(int x, int z) => Biomes[x + OVERSAMPLE_RADIUS, z + OVERSAMPLE_RADIUS];
        //public void SetBiome(int x, int z, IBiome biome) => Biomes[x + OVERSAMPLE_RADIUS, z + OVERSAMPLE_RADIUS] = biome;

        public IBiome GetBiomeOversampled(int x, int z) => Biomes[x, z];
        public void SetBiomeOversampled(int x, int z, IBiome biome) => Biomes[x, z] = biome;
    }
}
