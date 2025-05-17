using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.ChunkSerializers
{
    public class ChunkEncoder
    {
        IChunkEncoder serializer_0 = new NullSerializer();
        IChunkEncoder serializer_1 = new NoCompressionEncoder();
        IChunkEncoder serializer_2 = new RunLengthEncoder();

        public ChunkData Deserialize(ReadOnlySpan<byte> chunkData, int algorithm)
        {
            return GetSerializer(algorithm).Deserialize(chunkData);
        }

        public ReadOnlySpan<byte> Serialize(ChunkData chunkData, out int algorithm)
        {
            if (chunkData.IsEmpty())
            {
                algorithm = 0;
                return ReadOnlySpan<byte>.Empty;
            }

            algorithm = 2;
            return GetSerializer(algorithm).Serialize(chunkData);
        }

        private IChunkEncoder GetSerializer(int algorithm)
        {
            return algorithm switch
            {
                0 => serializer_0,
                1 => serializer_1,
                2 => serializer_2,
                _ => serializer_1,
            };
        }
    }
}
