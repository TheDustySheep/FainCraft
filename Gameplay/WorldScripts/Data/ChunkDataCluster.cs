using System;
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

    public VoxelData GetVoxel(uint x,  uint y, uint z)
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

    public void GetVoxels(uint x, uint y, uint z, Span<VoxelData> data)
    {
        int index = 0;
        for (int z_off = 0; z_off < 3; z_off++)
        {
            int z_local = (int)z + z_off - 1;

            for (int y_off = 0; y_off < 3; y_off++)
            {
                int y_local = (int)y + y_off - 1;

                for (int x_off = 0; x_off < 3; x_off++)
                {
                    int x_local = (int)x + x_off - 1;

                    int chunkIndex = 13 +
                        (x_local >> CHUNK_SIZE_POWER) +
                        (y_local >> CHUNK_SIZE_POWER) * 3 +
                        (z_local >> CHUNK_SIZE_POWER) * 9;

                    int localIndex = ChunkIndex
                    (
                        x_local + CHUNK_SIZE & CHUNK_SIZE_MASK,
                        y_local + CHUNK_SIZE & CHUNK_SIZE_MASK,
                        z_local + CHUNK_SIZE & CHUNK_SIZE_MASK
                    );

                    var chunkData = nChunks[chunkIndex];

                    data[index] = chunkData.VoxelData[localIndex];

                    index++;
                }
            }
        }
    }

    public void NewGetVoxels(int x, int y, int z, Span<VoxelData> data)
    {
        int index = 0;
        for (int z_off = -1; z_off < 2; z_off++)
        {
            int z_local = z + z_off;

            for (int y_off = -1; y_off < 2; y_off++)
            {
                int y_local = y + y_off;

                for (int x_off = -1; x_off < 2; x_off++)
                {
                    int x_local = x + x_off;

                    int chunkIndex = 13 +
                        (x_local >> CHUNK_SIZE_POWER) +
                        (y_local >> CHUNK_SIZE_POWER) * 3 +
                        (z_local >> CHUNK_SIZE_POWER) * 9;

                    int localIndex =
                        ((x_local + CHUNK_SIZE) & CHUNK_SIZE_MASK) +
                        ((y_local + CHUNK_SIZE) & CHUNK_SIZE_MASK) * CHUNK_SIZE +
                        ((z_local + CHUNK_SIZE) & CHUNK_SIZE_MASK) * CHUNK_SIZE * 2;

                    var chunkData = nChunks[chunkIndex];

                    data[index] = chunkData.VoxelData[localIndex];

                    index++;
                }
            }
        }
    }
}
