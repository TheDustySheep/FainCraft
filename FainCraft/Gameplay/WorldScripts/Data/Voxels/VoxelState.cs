namespace FainCraft.Gameplay.WorldScripts.Data.Voxels;
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

    public override string ToString()
    {
        return $"Voxel State Idx: {Index}";
    }
}
