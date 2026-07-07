using FainCraft.Gameplay.WorldScripts.Data.Chunks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.ChunkSerializers
{
    public class ChunkEncoder
    {
        IChunkEncoder serializer_0 = new NullSerializer();
        IChunkEncoder serializer_1 = new NoCompressionEncoder();
        IChunkEncoder serializer_2 = new RunLengthEncoder();

        public ChunkData Decode(ReadOnlySpan<byte> chunkData, short algorithm)
        {
            return GetEncoder(algorithm).Deserialize(chunkData);
        }

        public ReadOnlySpan<byte> Encode(ChunkData chunkData, out byte algorithm)
        {
            if (chunkData.IsEmpty())
            {
                algorithm = 0;
                return ReadOnlySpan<byte>.Empty;
            }

            algorithm = 2;
            return GetEncoder(algorithm).Serialize(chunkData);
        }

        private IChunkEncoder GetEncoder(int algorithm)
        {
            return algorithm switch
            {
                0 => serializer_0,
                1 => serializer_1,
                2 => serializer_2,
                _ => throw new NotImplementedException($"Encoder not implemented {algorithm}"),
            };
        }
    }
}
