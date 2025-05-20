using FainCraft.Gameplay.WorldScripts.Coords;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld.NoiseMaps
{
    internal class NoiseMapMountains
    {
        readonly FastNoiseLite noise;

        public NoiseMapMountains(int seed)
        {
            noise = new FastNoiseLite(seed + 4010);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            noise.SetFrequency(0.001f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(5);

            //ridge_noise.SetFractalGain(0.65f);
            //ridge_noise.SetFractalLacunarity(3.0f);
        }

        public float Sample(VoxelCoord2DGlobal coord)
        {
            return noise.GetNoise(coord.X, coord.Z);
        }
    }
}
