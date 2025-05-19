using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data;
public class ChunkDataClusterFast1 : IChunkDataCluster
{
    VoxelState[] _internalData = new VoxelState[34 * 34 * 34];

    public void SetData(ChunkData?[] nChunks)
    {
        int newSize = CHUNK_SIZE + 2;                   // 34

        // helper to pick which chunk index and local coord
        void Select(int global, out int local, out int cIdx)
        {
            int loc = global - 1;
            if (loc < 0) { cIdx = 0; local = CHUNK_SIZE - 1; }
            else if (loc >= CHUNK_SIZE) { cIdx = 2; local = 0; }
            else { cIdx = 1; local = loc; }
        }

        for (int gy = 0; gy < newSize; gy++)
            for (int gz = 0; gz < newSize; gz++)
                for (int gx = 0; gx < newSize; gx++)
                {
                    // which neighbour chunk along each axis, and local coordinate in it
                    Select(gx, out int lx, out int cx);
                    Select(gz, out int lz, out int cz);
                    Select(gy, out int ly, out int cy);

                    // calculate 1D index into neighbourChunks[]
                    int neighbourIndex = cx + cz * 3 + cy * 9;
                    var chunk = nChunks[neighbourIndex];
                    // flat index in that chunk (X→Z→Y)
                    int idxInChunk = lx + lz * CHUNK_SIZE + ly * CHUNK_SIZE * CHUNK_SIZE;

                    // flat index in output (also X→Z→Y, but size=PADDED_SIZE)
                    int outIdx = gx + gz * newSize + gy * newSize * newSize;
                    _internalData[outIdx] = chunk![idxInChunk];
                }
    }

    /*
    public void SetData(ChunkData[] chunkDatas)
    {
        Span<VoxelState> dst = _internalData.AsSpan();
        Span<VoxelState> src;

        // (-1, 0, 0)
        src = chunkDatas[4].AsSpan();
        for (int z = 0; z < CHUNK_SIZE; z++)
        {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                Span<VoxelState> s_ctr = src.Slice(ChunkIndex ( 0, y, 0), 32);
                Span<VoxelState> s_dst = dst.Slice(PaddedIndex( 1, y, 0), 32);
                s_ctr.CopyTo(s_dst);
            }
        }

        // ( 1, 0, 0)
        src = chunkDatas[22].AsSpan();
        for (int z = 0; z < CHUNK_SIZE; z++)
        {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                Span<VoxelState> s_ctr = src.Slice(ChunkIndex ( 0, y, 0), 32);
                Span<VoxelState> s_dst = dst.Slice(PaddedIndex( 1, y, 33), 32);
                s_ctr.CopyTo(s_dst);
            } 
        }

        // ( 0, 0,-1)
        src = chunkDatas[4].AsSpan();
        for (int y = 0; y < CHUNK_SIZE; y++)
        {
            Span<VoxelState> s_ctr = src.Slice(ChunkIndex ( 0, y, 0), 32);
            Span<VoxelState> s_dst = dst.Slice(PaddedIndex( 1, y, 0), 32);
            s_ctr.CopyTo(s_dst);
        }

        // ( 0, 0, 1)
        src = chunkDatas[22].AsSpan();
        for (int y = 0; y < CHUNK_SIZE; y++)
        {
            Span<VoxelState> s_ctr = src.Slice(ChunkIndex ( 0, y, 0), 32);
            Span<VoxelState> s_dst = dst.Slice(PaddedIndex( 1, y,33), 32);
            s_ctr.CopyTo(s_dst);
        }

        // ( 0, -1, 0)
        src = chunkDatas[4].AsSpan();
        for (int z = 0; z < CHUNK_SIZE; z++)
        {
            Span<VoxelState> s_ctr = src.Slice(ChunkIndex ( 0, 0,  z), 32);
            Span<VoxelState> s_dst = dst.Slice(PaddedIndex( 1, 0,z+1), 32);
            s_ctr.CopyTo(s_dst);
        }

        // ( 0,  1, 0)
        src = chunkDatas[22].AsSpan();
        for (int z = 0; z < CHUNK_SIZE; z++)
        {
            Span<VoxelState> s_ctr = src.Slice(ChunkIndex ( 0, 0,  z), 32);
            Span<VoxelState> s_dst = dst.Slice(PaddedIndex( 1,33,z+1), 32);
            s_ctr.CopyTo(s_dst);
        }

        // ( 0, 0, 0)
        src = chunkDatas[13].AsSpan();
        for (int z = 0; z < CHUNK_SIZE; z++)
        {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                Span<VoxelState> s_ctr = src.Slice(ChunkIndex (0, y, z), 32);
                Span<VoxelState> s_dst = dst.Slice(PaddedIndex(1, y, z+1), 32);
                s_ctr.CopyTo(s_dst);
            }
        }
    }
    */

    public void SetData(ChunkData chunkData)
    {

    }

    public VoxelState GetVoxelCenter(uint x, uint y, uint z)
    {
        return default;
    }

    public VoxelState GetVoxel(int x, int y, int z)
    {
        return default;
    }


    const int PADDED_SIZE = 34;
    const int PADDED_AREA = 34*34;
    static int PaddedIndex(int x, int y, int z) => x + z * PADDED_SIZE + y * PADDED_AREA;
}
