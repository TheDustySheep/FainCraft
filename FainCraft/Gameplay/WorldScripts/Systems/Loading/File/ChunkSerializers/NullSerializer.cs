using FainCraft.Gameplay.WorldScripts.Data.ChunkDataArrays;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.ChunkSerializers
{
    public class NullSerializer : IChunkEncoder
    {
        public ChunkDataRef Deserialize(ReadOnlySpan<byte> chunkData)
        {
            return new ChunkDataRef(new ChunkDataArray());
        }

        public ReadOnlySpan<byte> Serialize(ChunkDataRef chunkData)
        {
            return ReadOnlySpan<byte>.Empty;
        }
    }
}
