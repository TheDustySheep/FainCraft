using FainCraft.Gameplay.WorldScripts.Data.Chunks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.ChunkSerializers
{
    public class NullSerializer : IChunkEncoder
    {
        public ChunkData Deserialize(ReadOnlySpan<byte> chunkData)
        {
            return new ChunkData();
        }

        public ReadOnlySpan<byte> Serialize(ChunkData chunkData)
        {
            return ReadOnlySpan<byte>.Empty;
        }
    }
}
