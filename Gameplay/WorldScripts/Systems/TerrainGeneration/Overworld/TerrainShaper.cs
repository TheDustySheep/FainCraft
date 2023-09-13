using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainEngine_v2.Utils;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld;
internal class TerrainShaper
{
    readonly FastNoiseLite overhangNoise = new();
    readonly FastNoiseLite overhangNoiseMask = new();

    readonly HeightMapGenerator heightMapGenerator = new();

    public TerrainShaper()
    {
        overhangNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        overhangNoise.SetFrequency(0.01f);

        overhangNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
        overhangNoise.SetFractalWeightedStrength(0.5f);
        overhangNoise.SetFractalOctaves(4);

        overhangNoiseMask.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        overhangNoiseMask.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance);
        overhangNoiseMask.SetFrequency(0.01f);
    }

    public void ShapeTerrain(GenerationData data)
    {
        heightMapGenerator.GenerateHeightMap(data);
        for (int y = 0; y < REGION_Y_TOTAL_COUNT; y++)
        {
            GenerateChunk(data.Chunks[y], data,
                new ChunkCoord(
                    data.RegionCoord.X,
                    y - REGION_Y_NEG_COUNT,
                    data.RegionCoord.Z)
                );
        }
    }

    private void GenerateChunk(ChunkData chunk, GenerationData data, ChunkCoord chunkCoord)
    {
        var stone = data.Indexer.GetVoxel("Stone");

        GlobalVoxelCoord chunkGlobalCoord = (GlobalVoxelCoord)chunkCoord;

        for (int z = 0; z < CHUNK_SIZE; z++)
        {
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    GlobalVoxelCoord globalCoord = chunkGlobalCoord + new GlobalVoxelCoord(x, y, z);

                    int height = data.Continentalness.GetHeight(x, z);

                    if (globalCoord.Y <= height)
                    {
                        chunk[x, y, z] = stone;
                    }
                    else
                    {
                        float cutoff = overhangNoiseMask.GetNoise(globalCoord.X, globalCoord.Z) * 0.01f;
                        cutoff += MathUtils.InvLerp(height, height + 48, globalCoord.Y);

                        if (overhangNoise.GetNoise(globalCoord.X, globalCoord.Y, globalCoord.Z) > cutoff)
                        {
                            chunk[x, y, z] = stone;
                        }
                    }
                }
            }
        }
    }
}
