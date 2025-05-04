using FainCraft.Gameplay.WorldScripts.Data;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.ChunkSerializers
{
    public class NoCompressionSerializer : IChunkSerializer
    {
        public ChunkData Deserialize(ReadOnlySpan<byte> chunkData)
        {
            var voxels = MemoryMarshal.Cast<byte, VoxelState>(chunkData).ToArray();
            return new ChunkData(voxels);
        }

        public ReadOnlySpan<byte> Serialize(ChunkData chunkData)
        {
            Span<VoxelState> voxels = chunkData.VoxelData.AsSpan();
            return MemoryMarshal.Cast<VoxelState, byte>(voxels);
        }
    }
}
