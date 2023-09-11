namespace FainCraft.Gameplay.WorldScripts.Data.ChunkDataArrays;
internal interface IChunkDataArray
{
    public bool IsEmpty { get; }
    public ushort GetID(uint index);
    public void SetID(uint index, ushort newID);
    public bool ContainsID(ushort id);
}
