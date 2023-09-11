using FainCraft.Gameplay.WorldScripts.Core;
using static FainCraft.Gameplay.WorldScripts.Core.CoordConversions;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data;
public class ChunkData
{
    public VoxelData[] VoxelData { get; private set; } = new VoxelData[CHUNK_VOLUME];

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

    public VoxelData this[int index]
    {
        get => VoxelData[index];
        set => VoxelData[index] = value;
    }

    public VoxelData this[uint index]
    {
        get => VoxelData[index];
        set => VoxelData[index] = value;
    }

    public VoxelData this[int x, int y, int z]
    {
        get => VoxelData[ConvertToArrayIndex(x, y, z)];
        set => VoxelData[ConvertToArrayIndex(x, y, z)] = value;
    }

    public VoxelData this[uint x, uint y, uint z]
    {
        get => VoxelData[ConvertToArrayIndex(x, y, z)];
        set => VoxelData[ConvertToArrayIndex(x, y, z)] = value;
    }

    public VoxelData this[LocalVoxelCoord localVoxelCoord]
    {
        get => VoxelData[localVoxelCoord.VoxelIndex];
        set => VoxelData[localVoxelCoord.VoxelIndex] = value;
    }
}