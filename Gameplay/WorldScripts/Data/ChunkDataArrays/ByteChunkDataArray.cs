using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data.ChunkDataArrays;
internal class ByteChunkDataArray : IChunkDataArray
{
    readonly byte[] ids = new byte[CHUNK_VOLUME];

    public bool IsEmpty => false;

    public bool ContainsID(ushort id)
    {
        return ids.Contains((byte)id);
    }

    public ushort GetID(uint index)
    {
        return ids[index];
    }

    public void SetID(uint index, ushort newID)
    {
        ids[index] = (byte)newID;
    }
}
