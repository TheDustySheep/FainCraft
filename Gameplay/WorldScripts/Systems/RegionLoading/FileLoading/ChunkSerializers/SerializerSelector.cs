using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.ChunkSerializers
{
    public class SerializerSelector
    {
        IChunkSerializer serializer_0 = new NullSerializer();
        IChunkSerializer serializer_1 = new NoCompressionSerializer();
        IChunkSerializer serializer_2 = new RunLengthEncodingSerializer();

        public ChunkData Deserialize(int algorithm, ReadOnlySpan<byte> chunkData)
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

        private IChunkSerializer GetSerializer(int algorithm)
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
