using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data.ChunkDataArrays;
internal class ChunkDataArray : IChunkDataStorage
{
    readonly VoxelState[] _data = new VoxelState[CHUNK_VOLUME];

    public ChunkDataArray()
    {
        _data = new VoxelState[CHUNK_VOLUME];
    }

    public ChunkDataArray(VoxelState[] voxels)
    {
        if (voxels.Length != CHUNK_VOLUME)
            throw new ArgumentException($"Length of voxels array was incorrect. [{voxels.Length}/{CHUNK_VOLUME}]");
    }

    public bool IsEmpty()
    {
        ReadOnlySpan<VoxelState> span = _data.AsSpan();

        ulong sum = 0;
        foreach (var state in span)
            sum += state.Index;
        return sum == 0;
    }

    public void CopyTo(Span<VoxelState> destination)
    {
        _data.CopyTo(destination);
    }

    public VoxelState Get(uint index)
    {
        return _data[index];
    }

    public void Set(uint index, VoxelState value)
    {
        _data[index] = value;
    }
}
