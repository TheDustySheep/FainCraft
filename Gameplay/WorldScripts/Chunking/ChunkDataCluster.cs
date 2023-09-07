using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Chunking;
internal class ChunkDataCluster
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

    public void GetVoxels(int x, int y, int z, VoxelData[] data)
    {
        int index = 0;
        for (int z_off = 0; z_off < 3; z_off++)
        {
            int z_local = z + z_off - 1;

            for (int y_off = 0; y_off < 3; y_off++)
            {
                int y_local = y + y_off - 1;

                for (int x_off = 0; x_off < 3; x_off++)
                {
                    int x_local = x + x_off - 1;

                    int chunkIndex = 13 +
                        (x_local >> CHUNK_SIZE_POWER) +
                        (y_local >> CHUNK_SIZE_POWER) * 3 +
                        (z_local >> CHUNK_SIZE_POWER) * 9;

                    int localIndex = ConvertToIndex
                    (
                        (x_local + CHUNK_SIZE) & CHUNK_SIZE_MASK,
                        (y_local + CHUNK_SIZE) & CHUNK_SIZE_MASK,
                        (z_local + CHUNK_SIZE) & CHUNK_SIZE_MASK
                    );

                    var chunkData = nChunks[chunkIndex];

                    data[index] = chunkData.VoxelData[localIndex];

                    index++;
                }
            }
        }
    }
}
