using FainCraft.Gameplay.WorldScripts.Core;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data;
public class ChunkData
{
    public VoxelState[] VoxelData { get; private set; }

    public ChunkData()
    {
        VoxelData = new VoxelState[CHUNK_VOLUME];
    }

    public ChunkData(VoxelState[] states)
    {
        if (states.Length != CHUNK_VOLUME)
            throw new Exception($"Error loading voxel states. Array of voxels was not the expected length. {states.Length}/{CHUNK_VOLUME}");

        VoxelData = states;
    }

    public bool IsEmpty()
    {
        ReadOnlySpan<VoxelState> span = VoxelData.AsSpan();

        ulong sum = 0;
        foreach (var state in span)
            sum += state.Index;
        return sum == 0;
    }

    public void Clear()
    {
        Span<VoxelState> dst = VoxelData.AsSpan();
        dst.Clear();
    }

    public void CopyFrom(ChunkData other)
    {
        Span<VoxelState> dst = VoxelData.AsSpan();
        Span<VoxelState> src = other.VoxelData.AsSpan();
        src.CopyTo(dst);
    }

    public Span<VoxelState> AsSpan() => VoxelData.AsSpan();

    public static readonly ChunkData Empty = new();

    public VoxelState this[int index]
    {
        get => VoxelData[index];
        set => VoxelData[index] = value;
    }

    public VoxelState this[uint index]
    {
        get => VoxelData[index];
        set => VoxelData[index] = value;
    }

    public VoxelState this[int x, int y, int z]
    {
        get => VoxelData[ChunkIndex(x, y, z)];
        set => VoxelData[ChunkIndex(x, y, z)] = value;
    }

    public VoxelState this[uint x, uint y, uint z]
    {
        get => VoxelData[ChunkIndex(x, y, z)];
        set => VoxelData[ChunkIndex(x, y, z)] = value;
    }

    public VoxelState this[VoxelCoordLocal localVoxelCoord]
    {
        get => VoxelData[localVoxelCoord.VoxelIndex];
        set => VoxelData[localVoxelCoord.VoxelIndex] = value;
    }
}