using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Utils;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;
using static FainEngine_v2.Utils.EasingFunctions;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.HeightMaps
{
    internal class ContinentGenerator
    {
        readonly FastNoiseLite noise;

        public ContinentGenerator(int seed)
        {
            noise = new FastNoiseLite(seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            noise.SetFrequency(0.0034f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(2);
        }

        public void Generate(Array2D<float> data, RegionCoord regionCoord)
        {
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    var coord = new GlobalVoxelCoord(regionCoord, new LocalVoxelCoord(x, 0, z));
                    float val = noise.GetNoise(coord.X * 0.1f, coord.Z * 0.1f);

                    val = RemapNeg1_1To0_1(val);
                    val = EaseInOutQuint(val);
                    val = Clamp01(val);

                    data[x, z] = val;
                }
            }
        }
    }
}
