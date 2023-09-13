using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Utils;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld;
internal class HeightMapGenerator
{
    readonly FastNoiseLite cellularNoise;
    readonly FastNoiseLite simplexNoise;
    readonly FastNoiseLite smallpeaksNoise;

    const int groundHeight = 32;

    public HeightMapGenerator()
    {
        cellularNoise = new FastNoiseLite();

        cellularNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        cellularNoise.SetFrequency(0.01f);
        cellularNoise.SetCellularJitter(1);
        cellularNoise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);

        cellularNoise.SetFractalType(FastNoiseLite.FractalType.Ridged);
        cellularNoise.SetFractalOctaves(4);
        cellularNoise.SetFractalWeightedStrength(0.5f);

        simplexNoise = new FastNoiseLite();

        simplexNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        simplexNoise.SetFrequency(0.002f);

        simplexNoise.SetFractalType(FastNoiseLite.FractalType.Ridged);
        simplexNoise.SetFractalOctaves(4);
        simplexNoise.SetFractalWeightedStrength(0.5f);

        smallpeaksNoise = new FastNoiseLite();
        smallpeaksNoise.SetFrequency(0.03f);
    }

    public void GenerateHeightMap(GenerationData data)
    {
        var img = data.Continentalness.Image;
        Span<ushort> heightMap = data.Continentalness.Heights;

        RegionCoord regionCoord = data.RegionCoord;
        int start_x = regionCoord.Global_Voxel_X - HeightMap.BORDER;
        int start_z = regionCoord.Global_Voxel_Z - HeightMap.BORDER;

        for (int x = 0; x < HeightMap.MAP_SIZE; x++)
        {
            int g_x = start_x + x;

            for (int z = 0; z < HeightMap.MAP_SIZE; z++)
            {
                int g_z = start_z + z;

                img[x, z] = new HalfSingle(CalculateGroundHeight(g_x, g_z));
            }
        }

        img.Mutate(i => i.BoxBlur(HeightMap.BORDER));

        for (int x = 0; x < HeightMap.MAP_SIZE; x++)
        {
            int g_x = start_x + x;

            for (int z = 0; z < HeightMap.MAP_SIZE; z++)
            {
                int g_z = start_z + z;

                heightMap[z * HeightMap.MAP_SIZE + x] = (ushort)(short)(img[x, z].ToSingle() + smallpeaksNoise.GetNoise(g_x, g_z));
            }
        }
    }

    private float CalculateGroundHeight(int x, int z)
    {
        float simplexVal = simplexNoise.GetNoise(x, z);

        float total = simplexVal * 32;

        return MathF.Max(0, total + groundHeight);
    }
}
