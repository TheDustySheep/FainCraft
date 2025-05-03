using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.HeightGenerators
{
    internal class SimplexFBMTerrain : ITerrain
    {
        FastNoiseLite noise;

        float _baseHeight;
        float _variation;
        float _minHeight;
        float _maxHeight;

        public SimplexFBMTerrain(
            int seed,
            float baseHeight,
            float variation,
            float frequency,
            float minHeight = -10000,
            float maxHeight = 10000,
            int octaves = 5,
            float fbmGain = 0.5f,
            float fbmLacunarity = 2.0f)
        {
            _baseHeight = baseHeight;
            _variation = variation;

            _minHeight = minHeight;
            _maxHeight = maxHeight;

            noise = new FastNoiseLite(seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFrequency(frequency);
            noise.SetFractalOctaves(octaves);
            noise.SetFractalGain(fbmGain);
            noise.SetFractalLacunarity(fbmLacunarity);
        }

        public float Generate(RegionMaps maps, VoxelCoord2DGlobal coord)
        {
            float val = noise.GetNoise(coord.X, coord.Z) * _variation + _baseHeight;
            val = MathF.Max(val, _minHeight);
            val = MathF.Min(val, _maxHeight);
            return val;
        }
    }
}
