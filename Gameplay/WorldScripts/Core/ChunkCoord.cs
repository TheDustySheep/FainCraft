using Silk.NET.Maths;
using System.Numerics;
using System.Runtime.CompilerServices;
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

    public ChunkCoord(RegionCoord regionCoord, int y)
    {
        X = regionCoord.X;
        Y = y;
        Z = regionCoord.Z;
    }

    public Vector3 GlobalCenter => GlobalMin + Extents;
    public readonly Vector3 GlobalMin =>
        new
        (
            ConvertToGlobalFromChunk(X),
            ConvertToGlobalFromChunk(Y),
            ConvertToGlobalFromChunk(Z)
        );

    public Vector3 Size = Vector3.One * CHUNK_SIZE;
    public Vector3 Extents = Vector3.One * CHUNK_SIZE * 0.5f;

    #region Conversions
    public static explicit operator VoxelCoordGlobal(ChunkCoord chunkCoord)
    {
        return ConvertToGlobalCoord(chunkCoord);
    }

    public static explicit operator RegionCoord(ChunkCoord chunkCoord)
    {
        return ConvertToRegionCoord(chunkCoord);
    }
    #endregion

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
