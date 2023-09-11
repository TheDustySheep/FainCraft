using Silk.NET.Maths;
using System.Numerics;
using static FainCraft.Gameplay.WorldScripts.Core.CoordConversions;

namespace FainCraft.Gameplay.WorldScripts.Core;

public struct GlobalVoxelCoord
{
    public int X;
    public int Y;
    public int Z;

    public GlobalVoxelCoord(LocalVoxelCoord local, ChunkCoord chunk)
    {
        X = ConvertToGlobalFromChunk(chunk.X) + local.X;
        Y = ConvertToGlobalFromChunk(chunk.Y) + local.Y;
        Z = ConvertToGlobalFromChunk(chunk.Z) + local.Z;
    }

    public GlobalVoxelCoord(RegionCoord regionCoord)
    {
        X = ConvertToGlobalFromChunk(regionCoord.X);
        Y = 0;
        Z = ConvertToGlobalFromChunk(regionCoord.Z);
    }

    public GlobalVoxelCoord(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public GlobalVoxelCoord(float x, float y, float z)
    {
        X = (int)MathF.Floor(x);
        Y = (int)MathF.Floor(y);
        Z = (int)MathF.Floor(z);
    }

    public GlobalVoxelCoord(Vector3 position)
    {
        X = (int)MathF.Floor(position.X);
        Y = (int)MathF.Floor(position.Y);
        Z = (int)MathF.Floor(position.Z);
    }

    public GlobalVoxelCoord(Vector3D<int> position)
    {
        X = position.X;
        Y = position.Y;
        Z = position.Z;
    }

    public static explicit operator ChunkCoord(GlobalVoxelCoord pos)
    {
        return ChunkCoord.CreateFromGlobalVoxel(pos.X, pos.Y, pos.Z);
    }

    public static explicit operator LocalVoxelCoord(GlobalVoxelCoord pos)
    {
        return LocalVoxelCoord.CreateFromGlobalVoxel(pos.X, pos.Y, pos.Z);
    }

    public static explicit operator GlobalVoxelCoord(Vector3D<int> pos)
    {
        return new GlobalVoxelCoord(pos.X, pos.Y, pos.Z);
    }

    public static explicit operator Vector3D<int>(GlobalVoxelCoord pos)
    {
        return new Vector3D<int>(pos.X, pos.Y, pos.Z);
    }

    public static explicit operator Vector3(GlobalVoxelCoord pos)
    {
        return new Vector3(pos.X, pos.Y, pos.Z);
    }

    public static GlobalVoxelCoord operator +(GlobalVoxelCoord a, ChunkCoord b)
    {
        var c = (GlobalVoxelCoord)b;
        return new GlobalVoxelCoord(a.X + c.X, a.Y + c.Y, a.Z + c.Z);
    }

    public static GlobalVoxelCoord operator +(GlobalVoxelCoord a, GlobalVoxelCoord b)
    {
        return new GlobalVoxelCoord(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static GlobalVoxelCoord operator -(GlobalVoxelCoord a, GlobalVoxelCoord b)
    {
        return new GlobalVoxelCoord(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static GlobalVoxelCoord operator *(GlobalVoxelCoord a, int b)
    {
        return new GlobalVoxelCoord(a.X * b, a.Y * b, a.Z * b);
    }

    public static GlobalVoxelCoord operator *(int b, GlobalVoxelCoord a)
    {
        return new GlobalVoxelCoord(a.X * b, a.Y * b, a.Z * b);
    }

    public static bool operator ==(GlobalVoxelCoord a, GlobalVoxelCoord b)
    {
        return
            a.X == b.X &&
            a.Y == b.Y &&
            a.Z == b.Z;
    }

    public static bool operator !=(GlobalVoxelCoord a, GlobalVoxelCoord b)
    {
        return
            a.X != b.X ||
            a.Y != b.Y ||
            a.Z != b.Z;
    }

    public override readonly string ToString()
    {
        return $"X: {X} Y: {Y} Z:{Z}";
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is GlobalVoxelCoord coord && this == coord;
    }

    public override readonly int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
    }
}
