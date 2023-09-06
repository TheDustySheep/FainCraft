using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Chunking.ChunkDataArrays;
internal class ShortChunkDataArray : IChunkDataArray
{
    readonly ushort[] ids = new ushort[CHUNK_VOLUME];

    public bool IsEmpty => false;

    public bool ContainsID(ushort id)
    {
        return ids.Contains(id);
    }

    public ushort GetID(uint index)
    {
        return ids[index];
    }

    public void SetID(uint index, ushort newID)
    {
        ids[index] = newID;
    }
}