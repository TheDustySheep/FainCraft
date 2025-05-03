using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.SurfaceDecoration;
using FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.SurfacePainting;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.Biomes.Types
{
    internal class BiomeHills : IBiome
    {
        public ISurfacePainter Painter { get; private set; }
        public ISurfaceDecorator Decorator { get; } = new NullDecorator();

        FastNoiseLite ridge_noise;
        FastNoiseLite peak_noise;

        public BiomeHills(ISurfacePainter decorator)
        {
            Painter = decorator;

            ridge_noise = new FastNoiseLite();
            ridge_noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            ridge_noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
            ridge_noise.SetFrequency(0.0015f);
            ridge_noise.SetFractalOctaves(4);

            peak_noise = new FastNoiseLite();
            peak_noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            peak_noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            peak_noise.SetFrequency(0.04f);
            peak_noise.SetFractalOctaves(3);
        }

        public float SampleHeight(RegionMaps maps, VoxelCoord2DGlobal coord)
        {
            float val = ridge_noise.GetNoise(coord.X, coord.Z);

            // Ridge peaks
            val = 1 - MathF.Abs(val);
            val = MathF.Pow(val, 2f);

            val += peak_noise.GetNoise(coord.X, coord.Z) * 0.06f;

            // Blend mountains
            float m_blend = maps.Mountains[coord.Local_X, coord.Local_Z];
            m_blend = EasingFunctions.Remap(m_blend, 0f, 0.2f, 0f, 1f);
            m_blend = EasingFunctions.Clamp(m_blend, 0f, 1f);
            val *= m_blend;

            return val * 120f + 4f;
        }
    }
}
