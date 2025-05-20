using Silk.NET.Maths;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Coords;
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
    public readonly uint ManhattenDistance(RegionCoord other)
    {
        return (uint)(Math.Abs(X - other.X) + Math.Abs(Z - other.Z));
    }

    public readonly uint OctileDistance(RegionCoord other)
    {
        int x2 = other.X;
        int z2 = other.Z;

        int dx = Math.Abs(X - x2);
        int dy = Math.Abs(Z - z2);

        // Number of diagonal steps is the smaller of dx, dy
        int diagonalSteps = Math.Min(dx, dy);
        // Remaining straight steps are the difference between dx and dy
        int straightSteps = Math.Abs(dx - dy);

        return (uint)(diagonalSteps * 14 + straightSteps * 10) / 10;
    }

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

    public static RegionCoord operator +(RegionCoord a, Vector2D<int> b)
    {
        return new RegionCoord(a.X + b.X, a.Z + b.Y);
    }

    public static RegionCoord operator -(RegionCoord a, Vector2D<int> b)
    {
        return new RegionCoord(a.X - b.X, a.Z - b.Y);
    }
    #endregion

    #region Overrides
    public override readonly string ToString()
    {
        return $"X:{X} Z:{Z}";
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Z); // Second half of the bits
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is RegionCoord other && other == this;
    }
    #endregion
}
