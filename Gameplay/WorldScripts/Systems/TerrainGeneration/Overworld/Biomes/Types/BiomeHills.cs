using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.SurfaceDecoration;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.Types
{
    internal class BiomeHills : IBiome
    {
        public ISurfacePainter Painter { get; private set; }
        public ISurfaceDecorator Decorator { get; } = new NullDecorator();

        FastNoiseLite noise;

        public BiomeHills(ISurfacePainter decorator)
        {
            Painter = decorator;

            noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
            noise.SetFrequency(0.01f);
            noise.SetFractalOctaves(4);
        }

        public float SampleHeight(RegionMaps maps, VoxelCoord2DGlobal coord)
        {
            float val = noise.GetNoise(coord.X, coord.Z);

            // Ridge peaks
            val = 1 - MathF.Abs(val);
            val = MathF.Pow(val, 2);

            // Smooth out peaks with Cos
            //val = 0.5f * MathF.Cos(MathF.PI * (1f - val)) + 0.5f;

            // Blend mountains
            float m_blend = maps.Mountains[coord.Local_X, coord.Local_Z];
            m_blend = EasingFunctions.Remap(m_blend, 0f, 0.2f, 0f, 1f);
            m_blend = EasingFunctions.Clamp(m_blend, 0f, 1f);
            val *= m_blend;

            // Blend continents
            //float c_blend = maps.Continents[coord.Local_X, coord.Local_Z];
            //c_blend = EasingFunctions.Remap(c_blend, -0.2f, -0.1f, 0f, 1f);
            //c_blend = EasingFunctions.Clamp(c_blend, 0f, 1f);
            //val *= c_blend;



            return val * 96f + 4f;
        }
    }
}
