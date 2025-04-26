using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Core;

public struct VoxelCoord2DGlobal
{
    public int X;
    public int Z;

    public readonly int Local_X => ConvertToLocalFromGlobal(X);
    public readonly int Local_Z => ConvertToLocalFromGlobal(Z);

    public VoxelCoord2DGlobal(int g_x, int g_z)
    {
        X = g_x;
        Z = g_z;
    }

    public VoxelCoord2DGlobal(RegionCoord regionCoord, int l_x, int l_z)
    {
        X = ConvertToGlobalFromChunk(regionCoord.X) + l_x;
        Z = ConvertToGlobalFromChunk(regionCoord.Z) + l_z;
    }

    #region Conversions
    public static explicit operator RegionCoord(VoxelCoord2DGlobal coord)
    {
        return new RegionCoord(
            ConvertToChunkFromGlobal(coord.X),
            ConvertToChunkFromGlobal(coord.Z)
        );
    }
    #endregion

    #region Operators
    public static VoxelCoord2DGlobal operator +(VoxelCoord2DGlobal a, VoxelCoord2DGlobal b)
    {
        return new VoxelCoord2DGlobal(a.X + b.X, a.Z + b.Z);
    }

    public static VoxelCoord2DGlobal operator -(VoxelCoord2DGlobal a, VoxelCoord2DGlobal b)
    {
        return new VoxelCoord2DGlobal(a.X - b.X, a.Z - b.Z);
    }

    public static bool operator ==(VoxelCoord2DGlobal a, VoxelCoord2DGlobal b)
    {
        return
            a.X == b.X &&
            a.Z == b.Z;
    }

    public static bool operator !=(VoxelCoord2DGlobal a, VoxelCoord2DGlobal b)
    {
        return
            a.X != b.X ||
            a.Z != b.Z;
    }
    #endregion

    #region Overrides
    public override readonly string ToString()
    {
        return $"X: {X} Z:{Z}";
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is VoxelCoord2DGlobal coord && this == coord;
    }

    public override readonly int GetHashCode()
    {
        return ((X & 0xFFFF) << 16) | (Z & 0xFFFF); // Second half of the bits
    }
    #endregion
}
