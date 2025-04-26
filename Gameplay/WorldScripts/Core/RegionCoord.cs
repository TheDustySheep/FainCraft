using Silk.NET.Maths;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Core;
public struct RegionCoord
{
    public int X;
    public int Z;

    public RegionCoord(int x, int z)
    {
        X = x;
        Z = z;
    }

    public readonly int Global_Voxel_X => ConvertToGlobalFromChunk(X);
    public readonly int Global_Voxel_Z => ConvertToGlobalFromChunk(Z);

    #region Conversions
    public static explicit operator VoxelCoordGlobal(RegionCoord chunkCoord)
    {
        return ConvertToGlobalCoord(chunkCoord);
    }

    public static explicit operator ChunkCoord(RegionCoord chunkCoord)
    {
        return ConvertToChunkCoord(chunkCoord);
    }
    #endregion

    #region Operators
    public static bool operator ==(RegionCoord a, RegionCoord b)
    {
        return a.X == b.X && a.Z == b.Z;
    }

    public static bool operator !=(RegionCoord a, RegionCoord b)
    {
        return a.X != b.X || a.Z != b.Z;
    }

    public static RegionCoord operator +(RegionCoord a, RegionCoord b)
    {
        return new RegionCoord(a.X + b.X, a.Z + b.Z);
    }

    public static RegionCoord operator -(RegionCoord a, RegionCoord b)
    {
        return new RegionCoord(a.X - b.X, a.Z - b.Z);
    }
    #endregion

    #region Overrides
    public override readonly string ToString()
    {
        return $"X:{X} Z:{Z}";
    }

    public override readonly int GetHashCode()
    {
        return ((X & 0xFFFF) << 16) | (Z & 0xFFFF); // Second half of the bits
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is RegionCoord other && other == this;
    }
    #endregion
}
