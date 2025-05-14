namespace FainCraft.Gameplay.WorldScripts.Data;

using static global::FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

public class RegionData : IRegionData
{
    public readonly ChunkData[] Chunks;

    public RegionData()
    {
        Chunks = new ChunkData[REGION_TOTAL_CHUNKS];
        for (int y = 0; y < REGION_TOTAL_CHUNKS; y++)
        {
            Chunks[y] = new ChunkData();
        }
    }

    public RegionData(ChunkData[] _chunks)
    {
        if (_chunks.Length != REGION_TOTAL_CHUNKS)
            throw new Exception($"Supplied region data array is incorrect. Expected {REGION_TOTAL_CHUNKS}. Recieved {_chunks.Length}.");
        Chunks = _chunks;
    }

    public static readonly RegionData Empty = new();

    public bool GetChunk(int c_y, out ChunkData chunkData)
    {
        c_y += REGION_NEG_CHUNKS;
        if (c_y < 0 || c_y > REGION_TOTAL_CHUNKS - 1)
        {
            chunkData = default!;
            return false;
        }
        chunkData = Chunks[c_y];
        return true;
    }

    public bool SetChunk(int c_y, ChunkData data)
    {
        c_y += REGION_NEG_CHUNKS;
        if (c_y < 0 || c_y > REGION_TOTAL_CHUNKS - 1)
            return false;

        Chunks[c_y] = data;
        return true;
    }

    public bool GetVoxel(int l_x, int i_y, int l_z, out VoxelState voxelState)
    {
        if (!GetChunk((i_y >> 5) - REGION_NEG_CHUNKS, out var chunk))
        {
            voxelState = default;
            return false;
        }

        int l_y = ConvertToLocalFromGlobal(i_y);
        voxelState = chunk[l_x, l_y, l_z];
        return true;
    }

    public void SetVoxel(int l_x, int i_y, int l_z, VoxelState voxelState)
    {
        if (!GetChunk((i_y >> 5) - REGION_NEG_CHUNKS, out var chunk))
            return;

        int l_y = ConvertToLocalFromGlobal(i_y);
        chunk[l_x, l_y, l_z] = voxelState;
    }
}
