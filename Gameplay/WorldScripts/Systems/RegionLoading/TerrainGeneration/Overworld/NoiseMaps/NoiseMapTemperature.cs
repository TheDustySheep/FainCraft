﻿using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration.Overworld.NoiseMaps
{
    internal class NoiseMapTemperature
    {
        readonly FastNoiseLite noise;

        public NoiseMapTemperature(int seed)
        {
            noise = new FastNoiseLite(seed + 8111);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            noise.SetFrequency(0.001f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(3);

            //ridge_noise.SetFractalGain(0.65f);
            //ridge_noise.SetFractalLacunarity(3.0f);
        }

        public float Sample(VoxelCoord2DGlobal coord)
        {
            return noise.GetNoise(coord.X, coord.Z);
        }
    }
}
