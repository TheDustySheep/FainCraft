using FainCraft.Gameplay.WorldScripts.Chunking.ChunkDataArrays;
using FainCraft.Gameplay.WorldScripts.Core;
using System.Runtime.CompilerServices;
using static FainCraft.Gameplay.WorldScripts.Core.CoordConversions;

namespace FainCraft.Gameplay.WorldScripts.Chunking;
internal class ComplexChunkData
{
    IChunkDataArray dataArray = new EmptyChunkDataArray();
    uint indexedCount = 1;
    Dictionary<uint, ushort> remap1 = new Dictionary<uint, ushort>() { { 0, 0 } };
    Dictionary<ushort, uint> remap2 = new Dictionary<ushort, uint>() { { 0, 0 } };

    public bool IsOnlyAir()
    {
        return dataArray.IsEmpty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private VoxelData GetVoxel(int index)
    {
        ushort dataArrayId = dataArray.GetID((uint)index);

        return new VoxelData()
        {
            Index = remap2[dataArrayId],
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetVoxel(int index, VoxelData newData)
    {
        uint uIndex = (uint)index;

        ushort oldLocalID = dataArray.GetID(uIndex);
        uint oldVoxelID = remap2[oldLocalID];
        uint newVoxelID = newData.Index;

        // No changes need to be made
        if (oldVoxelID == newVoxelID)
            return;

        if (remap1.TryGetValue(newVoxelID, out ushort newLocalID))
        {
            dataArray.SetID(uIndex, newLocalID);

            // If the old id still exists
            if (dataArray.ContainsID(oldLocalID))
            {
                return;
            }
            else
            {
                // Dont remove air
                if (oldVoxelID == 0)
                    return;

                // TODO Remap and downsize array
                indexedCount--;
                if (indexedCount == 1)
                {

                }
            }
        }
        else
        {
            // Add new remap into both dicts
            for (ushort i = 1; i < ushort.MaxValue; i++)
            {
                if (!remap2.ContainsKey(i))
                {
                    remap1.Add(newVoxelID, i);
                    remap2.Add(i, newVoxelID);
                    newLocalID = i;
                    indexedCount++;
                    break;
                }
            }

            dataArray.SetID(uIndex, newLocalID);

            // TODO Remap
        }
    }

    public VoxelData this[int index]
    {
        get => GetVoxel(index);
        set => SetVoxel(index, value);
    }

    public VoxelData this[uint index]
    {
        get => GetVoxel((int)index);
        set => SetVoxel((int)index, value);
    }

    public VoxelData this[int x, int y, int z]
    {
        get => GetVoxel(ConvertToArrayIndex(x, y, z));
        set => SetVoxel(ConvertToArrayIndex(x, y, z), value);
    }

    public VoxelData this[uint x, uint y, uint z]
    {
        get => GetVoxel(ConvertToArrayIndex(x, y, z));
        set => SetVoxel(ConvertToArrayIndex(x, y, z), value);
    }

    public VoxelData this[LocalVoxelCoord localVoxelCoord]
    {
        get => GetVoxel(localVoxelCoord.VoxelIndex);
        set => SetVoxel(localVoxelCoord.VoxelIndex, value);
    }
}
