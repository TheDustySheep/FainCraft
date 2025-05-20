using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.ChunkSerializers
{
    public interface IChunkEncoder
    {
        public ChunkData Deserialize(ReadOnlySpan<byte> chunkData);
        public ReadOnlySpan<byte> Serialize(ChunkData chunkData);
    }
}
