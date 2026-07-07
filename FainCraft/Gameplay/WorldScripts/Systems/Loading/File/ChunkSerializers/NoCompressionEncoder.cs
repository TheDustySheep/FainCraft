using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using System.Runtime.InteropServices;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.ChunkSerializers
{
    public class NoCompressionEncoder : IChunkEncoder
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
