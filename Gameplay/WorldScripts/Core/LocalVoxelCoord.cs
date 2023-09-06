using static FainCraft.Gameplay.WorldScripts.Core.CoordConversions;

namespace FainCraft.Gameplay.WorldScripts.Core;

public struct LocalVoxelCoord
{
    private ushort LocalIndex;

    public readonly int VoxelIndex => LocalIndex;

    public int X
    {
        get => LocalIndex & 31;
        set => LocalIndex = (ushort)(LocalIndex & ~31 | value & 31);
    }

    public int Y
    {
        get => LocalIndex >> 5 & 31;
        set => LocalIndex = (ushort)(LocalIndex & ~992 | (value & 31) << 5);
    }

    public int Z
    {
        get => LocalIndex >> 10 & 31;
        set => LocalIndex = (ushort)(LocalIndex & ~31744 | (value & 31) << 10);
    }

    public LocalVoxelCoord(int x, int y, int z)
    {
        LocalIndex = (ushort)ConvertToArrayIndex(x, y, z);
    }

    public LocalVoxelCoord(int index)
    {
        LocalIndex = (ushort)index;
    }

    public static LocalVoxelCoord CreateFromGlobalVoxel(int x, int y, int z)
    {
        return new LocalVoxelCoord(
            ConvertToLocalFromGlobal(x),
            ConvertToLocalFromGlobal(y),
            ConvertToLocalFromGlobal(z));
    }

    public override string ToString()
    {
        return $"X: {X} Y: {Y} Z:{Z}";
    }

    public override readonly int GetHashCode()
    {
        return LocalIndex;
    }
}
