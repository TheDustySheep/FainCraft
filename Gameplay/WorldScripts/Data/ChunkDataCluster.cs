using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data;
public class ChunkDataCluster
{
    readonly ChunkData[] nChunks = new ChunkData[27];

    public ChunkDataCluster()
    {
        for (int i = 0; i < 27; i++)
        {
            nChunks[i] = new ChunkData();
        }
    }

    public bool CenterEmpty => nChunks[13].IsEmpty();

    public void SetData(ChunkData?[] chunkDatas)
    {
        for (int i = 0; i < 27; i++)
        {
            var chunk = chunkDatas[i];

            if (chunk is null)
                nChunks[i].Clear();
            else
                nChunks[i].CopyFrom(chunk);
        }
    }

    public VoxelData GetCenterChunkVoxel(uint x, uint y, uint z)
    {
        uint localIndex = ChunkIndex(x, y, z);

        var chunkData = nChunks[13];

        return chunkData[localIndex];
    }

    public VoxelData GetVoxel(int x, int y, int z)
    {
        int chunkIndex = 13 +
            (x >> CHUNK_SIZE_POWER) +
            (y >> CHUNK_SIZE_POWER) * 3 +
            (z >> CHUNK_SIZE_POWER) * 9;

        int localIndex = ChunkIndex
        (
            x + CHUNK_SIZE & CHUNK_SIZE_MASK,
            y + CHUNK_SIZE & CHUNK_SIZE_MASK,
            z + CHUNK_SIZE & CHUNK_SIZE_MASK
        );

        var chunkData = nChunks[chunkIndex];
        return chunkData[localIndex];
    }
}
