namespace FainCraft.Gameplay.WorldScripts.Data.ChunkDataArrays;
internal class EmptyChunkDataArray : IChunkDataArray
{
    public bool IsEmpty => true;
    public bool ContainsID(ushort id) => id == 0;
    public ushort GetID(uint index) => default;
    public void SetID(uint index, ushort newID) { }
}
