namespace FainCraft.Gameplay.WorldScripts.Chunking;
public struct VoxelData
{
    public uint Index;

    public static bool operator ==(VoxelData a, VoxelData b)
    {
        return a.Index == b.Index;
    }

    public static bool operator !=(VoxelData a, VoxelData b)
    {
        return a.Index != b.Index;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is VoxelData data && data == this;
    }

    public override int GetHashCode()
    {
        return unchecked((int)Index);
    }
}
