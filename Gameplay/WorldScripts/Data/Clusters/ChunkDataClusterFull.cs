using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data.Clusters;
public class ChunkDataClusterFull : IChunkDataCluster
{
    readonly ChunkData[] nChunks = new ChunkData[27];

    public ChunkDataClusterFull()
    {
        for (int i = 0; i < 27; i++)
        {
            nChunks[i] = new ChunkData();
        }
    }

    public bool IsEmpty => nChunks[13].IsEmpty();

    public void SetData(ChunkData?[] chunkDatas)
    {
        for (int i = 0; i < 27; i++)
        {
            var dst = nChunks[i].AsSpan();

            var chunk = chunkDatas[i];
            if (chunk == null)
                dst.Clear();
            else
            {
                var src = chunk.AsSpan();
                src.CopyTo(dst);
            }

        }
    }

    public void SetData(ChunkData chunkData)
    {
        nChunks[13].CopyFrom(chunkData);

        for (int i = 0; i < 13; i++)
            nChunks[i].Clear();

        for (int i = 14; i < 27; i++)
            nChunks[i].Clear();
    }

    public VoxelState GetVoxel(int x, int y, int z)
    {
        int chunkIndex = 13 +
            (x >> CHUNK_SIZE_POWER) +
            (z >> CHUNK_SIZE_POWER) * 3 +
            (y >> CHUNK_SIZE_POWER) * 9;

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
