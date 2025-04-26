using FainCraft.Gameplay.WorldScripts.Core;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data;
public class ChunkData
{
    public VoxelState[] VoxelData { get; private set; } = new VoxelState[CHUNK_VOLUME];

    public bool IsEmpty()
    {
        ulong sum = 0;
        for (int i = 0; i < CHUNK_VOLUME; i++)
            sum += VoxelData[i].Index;
        return sum == 0;
    }

    public void Clear()
    {
        Array.Clear(VoxelData);
    }

    public void CopyFrom(ChunkData other)
    {
        other.VoxelData.AsSpan().CopyTo(VoxelData);
    }

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