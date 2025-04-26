namespace FainCraft.Gameplay.WorldScripts.Data;
public struct VoxelState
{
    public uint Index;
    
    public static bool operator ==(VoxelState a, VoxelState b)
    {
        return a.Index == b.Index;
    }

    public static bool operator !=(VoxelState a, VoxelState b)
    {
        return a.Index != b.Index;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is VoxelState data && data == this;
    }

    public override readonly int GetHashCode()
    {
        return unchecked((int)Index);
    }
}
