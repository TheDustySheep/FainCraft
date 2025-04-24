using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Utils;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.HeightMaps
{
    internal class RidgeGenerator
    {
        readonly FastNoiseLite ridgeNoise;
        readonly FastNoiseLite detailNoise;

        public RidgeGenerator(int seed)
        {
            ridgeNoise = new FastNoiseLite(seed);
            ridgeNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            ridgeNoise.SetFrequency(0.001f);
            ridgeNoise.SetFractalType(FastNoiseLite.FractalType.Ridged);
            ridgeNoise.SetFractalOctaves(5);

            detailNoise = new FastNoiseLite(seed);
            detailNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            detailNoise.SetFrequency(0.01f);
            detailNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            detailNoise.SetFractalOctaves(4);
            detailNoise.SetFractalLacunarity(2.2f);
            detailNoise.SetFractalGain(0.4f);
        }

        public void Generate(Array2D<float> data, RegionCoord regionCoord)
        {
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    var coord = new GlobalVoxelCoord(regionCoord, new LocalVoxelCoord(x, 0, z));
                    float val = ridgeNoise.GetNoise(coord.X, coord.Z);

                    val = 1 - MathF.Abs(val);
                    val = MathF.Pow(val, 4);

                    val += detailNoise.GetNoise(coord.X, coord.Z) * 0.2f;

                    data[x, z] = val;
                }
            }
        }
    }
}
