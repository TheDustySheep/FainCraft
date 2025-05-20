using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data.Clusters;
public class ChunkDataClusterFast2 : IChunkDataCluster
{
    VoxelState[] _internalData = new VoxelState[34 * 34 * 34];

    static readonly int[] CHUNK_LOOKUP = ChunkLookup();
    static readonly int[] VOXEL_LOOKUP = VoxelLookup();

    private static int[] ChunkLookup()
    {
        int[] lookup = new int[CHUNK_SIZE + 2];
        Span<int> dst = lookup.AsSpan();
        dst.Fill(1);
        dst[0] = 0;
        dst[33] = 2;
        return lookup;
    }
    private static int[] VoxelLookup()
    {
        int[] lookup = new int[CHUNK_SIZE + 2];
        lookup[0] = 31;
        for (int i = 0; i < CHUNK_SIZE; i++)
            lookup[i] = i;
        lookup[33] = 0;
        return lookup;
    }

    const int PADDED_SIZE = 34;
    public void SetData(ChunkData?[] nChunks)
    {
        _internalData.AsSpan().Clear();

        var ctr = nChunks[13];
        if (ctr == null || ctr.IsEmpty())
            return;

        for (int i = 0; i < 27; i++)
            nChunks[i] = nChunks[i] ?? ChunkData.Empty;

        for (int gy = 0; gy < PADDED_SIZE; gy++)
            for (int gz = 0; gz < PADDED_SIZE; gz++)
                for (int gx = 0; gx < PADDED_SIZE; gx++)
                {
                    int lx = VOXEL_LOOKUP[gx];
                    int ly = VOXEL_LOOKUP[gy];
                    int lz = VOXEL_LOOKUP[gz];

                    int cx = CHUNK_LOOKUP[gx];
                    int cy = CHUNK_LOOKUP[gy];
                    int cz = CHUNK_LOOKUP[gz];

                    int neighbourIndex = cx + cz * 3 + cy * 9;
                    var chunk = nChunks[neighbourIndex];

                    int outIdx = gx + gz * PADDED_SIZE + gy * PADDED_SIZE * PADDED_SIZE;

                    // Disabled the null check here because we're setting it earlier in the function
                    _internalData[outIdx] = chunk![ChunkIndex(lx, ly, lz)];
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
        throw new NotImplementedException();
    }

    public VoxelState GetVoxel(int x, int y, int z)
    {
        return _internalData[x + 1 + (z + 1) * 34 + (y + 1) * 34 * 34];
    }
}
