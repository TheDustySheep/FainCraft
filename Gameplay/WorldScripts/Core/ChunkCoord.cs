using Silk.NET.Maths;
using System.Numerics;
using System.Runtime.CompilerServices;
using static FainCraft.Gameplay.WorldScripts.Core.CoordConversions;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Core;

[Serializable]
public struct ChunkCoord
{
    public int X;
    public int Y;
    public int Z;

    public ChunkCoord(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public readonly Vector3 GlobalCorner => new(
        ConvertToGlobalFromChunk(X),
        ConvertToGlobalFromChunk(Y),
        ConvertToGlobalFromChunk(Z));

    public Vector3 GlobalCenter => GlobalCorner + Vector3.One * CHUNK_SIZE * 0.5f;
    public Vector2D<int> XZPosition => new(X, Z);

    public static ChunkCoord CreateFromGlobalVoxel(int x, int y, int z)
    {
        return new ChunkCoord(
            ConvertToChunkFromGlobal(x),
            ConvertToChunkFromGlobal(y),
            ConvertToChunkFromGlobal(z));
    }

    public static explicit operator RegionCoord(ChunkCoord coord)
    {
        return new RegionCoord(coord.X, coord.Z);
    }

    public static explicit operator GlobalVoxelCoord(ChunkCoord coord)
    {
        return new GlobalVoxelCoord(
            ConvertToGlobalFromChunk(coord.X),
            ConvertToGlobalFromChunk(coord.Y),
            ConvertToGlobalFromChunk(coord.Z));
    }

    public static explicit operator Vector3D<int>(ChunkCoord coord)
    {
        return new Vector3D<int>(coord.X, coord.Y, coord.Z);
    }

    public static ChunkCoord operator +(ChunkCoord a, ChunkCoord b)
    {
        return new ChunkCoord(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static ChunkCoord operator -(ChunkCoord a, ChunkCoord b)
    {
        return new ChunkCoord(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static ChunkCoord operator *(ChunkCoord a, int b)
    {
        return new ChunkCoord(a.X * b, a.Y * b, a.Z * b);
    }

    public static ChunkCoord operator *(int b, ChunkCoord a)
    {
        return new ChunkCoord(a.X * b, a.Y * b, a.Z * b);
    }

    public static bool operator ==(ChunkCoord a, ChunkCoord b)
    {
        return
            a.X == b.X &&
            a.Y == b.Y &&
            a.Z == b.Z;
    }

    public static bool operator !=(ChunkCoord a, ChunkCoord b)
    {
        return
            a.X != b.X ||
            a.Y != b.Y ||
            a.Z != b.Z;
    }

    public override string ToString()
    {
        return $"X: {X} Y: {Y} Z:{Z}";
    }

    public override bool Equals(object? obj)
    {
        return obj is ChunkCoord coord && this == coord;
    }

    public override int GetHashCode()
    {
        return CombineHashCodes(X, Y, Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CombineHashCodes(int h1, int h2, int h3)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2), h3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CombineHashCodes(int h1, int h2)
    {
        return (h1 << 5) + h1 ^ h2;
    }
}
