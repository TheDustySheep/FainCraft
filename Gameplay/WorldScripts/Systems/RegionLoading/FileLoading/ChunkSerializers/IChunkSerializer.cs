using FainCraft.Gameplay.WorldScripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.FileLoading.ChunkSerializers
{
    public interface IChunkSerializer
    {
        public ChunkData Deserialize(ReadOnlySpan<byte> chunkData);
        public ReadOnlySpan<byte> Serialize(ChunkData chunkData);
    }
}
