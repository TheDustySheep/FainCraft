using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Core;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration;
internal class TestTerrainGenerator : ITerrainGenerator
{
    public RegionData Generate(RegionCoord regionCoord)
    {
        ChunkData[] chunks = new ChunkData[REGION_Y_TOTAL_COUNT];

        for (int y = 0; y < REGION_Y_TOTAL_COUNT; y++)
        {
            chunks[y] = GenerateChunk(new ChunkCoord()
            {
                X = regionCoord.X,
                Y = y + REGION_Y_NEG_COUNT,
                Z = regionCoord.Z,
            });
        }

        return new RegionData(chunks);
    }

    private static ChunkData GenerateChunk(ChunkCoord chunkCoord)
    {
        var chunkData = new ChunkData();

        for (int y = 0; y < CHUNK_SIZE; y++)
        {
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    ushort index = 0;
                    if (y < 16)
                        index = 2;
                    else if (y == 16)
                        index = 1;

                    var data = new VoxelData()
                    {
                        Index = index,
                    };
                    chunkData[x, y, z] = data;
                }
            }

        }

        return chunkData;
    }
}
