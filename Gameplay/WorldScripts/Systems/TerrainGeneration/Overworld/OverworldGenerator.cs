using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainEngine_v2.Utils;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.BasicWorld;
internal class OverworldGenerator : ITerrainGenerator
{
    const int stoneDepth = 4;
    const int waterHeight = 36;
    const int groundHeight = 32;

    readonly VoxelIndexer indexer;

    public OverworldGenerator(VoxelIndexer indexer)
    {
        this.indexer = indexer;
    }

    public RegionData Generate(RegionCoord regionCoord)
    {
        ChunkData[] chunks = new ChunkData[REGION_Y_TOTAL_COUNT];

        for (int y = 0; y < REGION_Y_TOTAL_COUNT; y++)
        {
            chunks[y] = GenerateChunk(new ChunkCoord()
            {
                X = regionCoord.X,
                Y = y - REGION_Y_NEG_COUNT,
                Z = regionCoord.Z,
            });
        }

        return new RegionData(chunks);
    }

    private ChunkData GenerateChunk(ChunkCoord chunkCoord)
    {
#if DEBUG
        Stopwatch sw = Stopwatch.StartNew();
#endif
        FastNoiseLite noise = new FastNoiseLite();

        VoxelData Air   = new() { Index = indexer.GetIndex("Air") };
        VoxelData Grass = new() { Index = indexer.GetIndex("Grass") };
        VoxelData Dirt  = new() { Index = indexer.GetIndex("Dirt") };
        VoxelData Sand  = new() { Index = indexer.GetIndex("Sand") };
        VoxelData Stone = new() { Index = indexer.GetIndex("Stone") };
        VoxelData Water = new() { Index = indexer.GetIndex("Water") };

        GlobalVoxelCoord chunkVoxelCoord = (GlobalVoxelCoord)chunkCoord;

        var chunkData = new ChunkData();

        for (int c_z = 0; c_z < CHUNK_SIZE; c_z++)
        {
            int global_z = c_z + chunkVoxelCoord.Z;

            for (int c_x = 0; c_x < CHUNK_SIZE; c_x++)
            {
                int global_x = c_x + chunkVoxelCoord.X;

                int groundHeight = CalculateGroundHeight(global_x, global_z, noise);

                for (int c_y = 0; c_y < CHUNK_SIZE; c_y++)
                {
                    int global_y = c_y + chunkVoxelCoord.Y;

                    VoxelData data;

                    if (global_y > groundHeight)
                    {
                        if (global_y <= waterHeight)
                            data = Water;
                        else
                            data = Air;
                    }
                    else if (global_y == groundHeight)
                    {
                        if (global_y <= waterHeight)
                            data = Sand;
                        else
                            data = Grass;
                    }
                    else if (global_y > groundHeight - stoneDepth)
                    {
                        if (global_y <= waterHeight)
                            data = Sand;
                        else
                            data = Dirt;
                    }
                    else
                        data = Stone;

                    chunkData[c_x, c_y, c_z] = data;
                }
            }
        }

#if DEBUG
        sw.Stop();
        SystemDiagnostics.SubmitTerrainGeneration(sw.Elapsed);
#endif

        return chunkData;
    }

    private int CalculateGroundHeight(int x, int z, FastNoiseLite noise)
    {
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        noise.SetFrequency(0.002f);

        noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
        noise.SetFractalOctaves(4);
        noise.SetFractalWeightedStrength(0.5f);

        float heightVal = noise.GetNoise(x, z) * 32;

        return (int)heightVal + groundHeight;
    }
}
