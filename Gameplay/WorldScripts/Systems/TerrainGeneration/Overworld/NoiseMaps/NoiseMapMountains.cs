using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.HeightMaps
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

            //noise.SetFractalGain(0.65f);
            //noise.SetFractalLacunarity(3.0f);
        }

        public float Sample(VoxelCoord2DGlobal coord)
        {
            return noise.GetNoise(coord.X, coord.Z);
        }
    }
}
