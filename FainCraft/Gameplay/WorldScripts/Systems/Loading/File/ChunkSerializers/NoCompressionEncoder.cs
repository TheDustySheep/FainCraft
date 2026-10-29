using FainCraft.Gameplay.WorldScripts.Data.ChunkDataArrays;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using System.Runtime.InteropServices;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.ChunkSerializers
{
    public class NoCompressionEncoder : IChunkEncoder
    {
        public ChunkDataRef Deserialize(ReadOnlySpan<byte> chunkData)
        {
            var voxels = MemoryMarshal.Cast<byte, VoxelState>(chunkData).ToArray();
            return new ChunkDataRef(new ChunkDataArray(voxels));
        }

        public ReadOnlySpan<byte> Serialize(ChunkDataRef chunkData)
        {
            Span<VoxelState> voxels = new VoxelState[CHUNK_VOLUME];
            chunkData.CopyTo(voxels);
            return MemoryMarshal.Cast<VoxelState, byte>(voxels);
        }
    }
}
