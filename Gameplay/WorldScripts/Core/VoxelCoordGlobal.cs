using Silk.NET.Maths;
using System.Numerics;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Core;

public struct VoxelCoordGlobal
{
    public int X;
    public int Y;
    public int Z;

    public VoxelCoordGlobal(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public VoxelCoordGlobal(float x, float y, float z)
    {
        X = (int)MathF.Floor(x);
        Y = (int)MathF.Floor(y);
        Z = (int)MathF.Floor(z);
    }

    public VoxelCoordGlobal(Vector3 position)
    {
        X = (int)MathF.Floor(position.X);
        Y = (int)MathF.Floor(position.Y);
        Z = (int)MathF.Floor(position.Z);
    }

    public VoxelCoordGlobal(Vector3D<int> position)
    {
        X = position.X;
        Y = position.Y;
        Z = position.Z;
    }

    public VoxelCoordGlobal(ChunkCoord chunkCoord, VoxelCoordLocal localCoord)
    {
        this = (VoxelCoordGlobal)localCoord + chunkCoord;
    }

    public VoxelCoordGlobal(RegionCoord regionCoord, VoxelCoordLocal localCoord)
    {
        this = (VoxelCoordGlobal)localCoord + (VoxelCoordGlobal)regionCoord;
    }

    #region Conversions
    public VoxelCoordGlobal Offset(int dx, int dy, int dz)
    {
        return new VoxelCoordGlobal(X + dx, Y + dy, Z + dz);
    }

    public static explicit operator VoxelCoordLocal(VoxelCoordGlobal globalCoord)
    {
        return ConvertToLocalCoord(globalCoord);
    }

    public static explicit operator ChunkCoord(VoxelCoordGlobal globalCoord)
    {
        return ConvertToChunkCoord(globalCoord);
    }

    public static explicit operator RegionCoord(VoxelCoordGlobal globalCoord)
    {
        return ConvertToRegionCoord(globalCoord);
    }

    public static explicit operator Vector3D<int>(VoxelCoordGlobal globalCoord)
    {
        return new Vector3D<int>(globalCoord.X, globalCoord.Y, globalCoord.Z);
    }

    public static explicit operator Vector3(VoxelCoordGlobal pos)
    {
        return new Vector3(pos.X, pos.Y, pos.Z);
    }
    #endregion

    #region Operators
    public static VoxelCoordGlobal operator +(VoxelCoordGlobal a, ChunkCoord b)
    {
        var c = (VoxelCoordGlobal)b;
        return new VoxelCoordGlobal(a.X + c.X, a.Y + c.Y, a.Z + c.Z);
    }

    public static VoxelCoordGlobal operator +(VoxelCoordGlobal a, VoxelCoordGlobal b)
    {
        return new VoxelCoordGlobal(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static VoxelCoordGlobal operator -(VoxelCoordGlobal a, VoxelCoordGlobal b)
    {
        return new VoxelCoordGlobal(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static VoxelCoordGlobal operator *(VoxelCoordGlobal a, int b)
    {
        return new VoxelCoordGlobal(a.X * b, a.Y * b, a.Z * b);
    }

    public static VoxelCoordGlobal operator *(int b, VoxelCoordGlobal a)
    {
        return new VoxelCoordGlobal(a.X * b, a.Y * b, a.Z * b);
    }

    public static bool operator ==(VoxelCoordGlobal a, VoxelCoordGlobal b)
    {
        return
            a.X == b.X &&
            a.Y == b.Y &&
            a.Z == b.Z;
    }

    public static bool operator !=(VoxelCoordGlobal a, VoxelCoordGlobal b)
    {
        return
            a.X != b.X ||
            a.Y != b.Y ||
            a.Z != b.Z;
    }
    #endregion

    #region Overrides
    public override readonly string ToString()
    {
        return $"XPos_px: {X} YPox_px: {Y} Z:{Z}";
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is VoxelCoordGlobal coord && this == coord;
    }

    public override readonly int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
    }
    #endregion
}
