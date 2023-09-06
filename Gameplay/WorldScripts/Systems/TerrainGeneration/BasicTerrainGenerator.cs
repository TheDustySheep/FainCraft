using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainEngine_v2.Utils;
using System.Diagnostics;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal class BasicTerrainGenerator : ITerrainGenerator
{
    VoxelIndexer indexer;

    public BasicTerrainGenerator(VoxelIndexer indexer)
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
        var noise = new FastNoiseLite();

        VoxelData Air = new() { Index = indexer.GetIndex("Air") };
        VoxelData Grass = new() { Index = indexer.GetIndex("Grass") };
        VoxelData Dirt = new() { Index = indexer.GetIndex("Dirt") };

        GlobalVoxelCoord chunkVoxelCoord = (GlobalVoxelCoord)chunkCoord;

        var chunkData = new ChunkData();

        for (int c_z = 0; c_z < CHUNK_SIZE; c_z++)
        {
            for (int c_x = 0; c_x < CHUNK_SIZE; c_x++)
            {
                for (int c_y = 0; c_y < CHUNK_SIZE; c_y++)
                {
                    var coord = chunkVoxelCoord + new GlobalVoxelCoord(c_x, c_y, c_z);
                    int groundHeight = (int)(noise.GetNoise(coord.X, coord.Z) * 8) + 32;

                    VoxelData data;

                    if (coord.Y < groundHeight)
                        data = Dirt;
                    else if (coord.Y == groundHeight)
                        data = Grass;
                    else
                        data = Air;

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
}
