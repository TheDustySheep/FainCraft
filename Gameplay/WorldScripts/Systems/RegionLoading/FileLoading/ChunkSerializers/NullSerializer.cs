using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.ChunkSerializers
{
    public class NullSerializer : IChunkSerializer
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
