using Silk.NET.Maths;
using System.Numerics;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Core;

public struct VoxelCoordLocal
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
        get => LocalIndex >> 10 & 31;
        set => LocalIndex = (ushort)(LocalIndex & ~31744 | (value & 31) << 10);
    }

    public int Z
    {
        get => LocalIndex >> 5 & 31;
        set => LocalIndex = (ushort)(LocalIndex & ~992 | (value & 31) << 5);
    }

    public VoxelCoordLocal(int x, int y, int z)
    {
        LocalIndex = (ushort)ChunkIndex(x, y, z);
    }

    public VoxelCoordLocal(int index)
    {
        LocalIndex = (ushort)index;
    }

    #region Conversions
    public static explicit operator VoxelCoordGlobal(VoxelCoordLocal localCoord)
    {
        return ConvertToGlobalCoord(localCoord);
    }

    public static explicit operator Vector3D<int>(VoxelCoordLocal localCoord)
    {
        return new Vector3D<int>(localCoord.X, localCoord.Y, localCoord.Z);
    }

    public static explicit operator Vector3(VoxelCoordLocal localCoord)
    {
        return new Vector3(localCoord.X, localCoord.Y, localCoord.Z);
    }
    #endregion

    public override string ToString()
    {
        return $"X: {X} Y: {Y} Z:{Z}";
    }

    public override readonly int GetHashCode()
    {
        return LocalIndex;
    }
}
