using FainEngine_v2.Extensions;
using System.Numerics;
using static FainCraft.Gameplay.WorldScripts.Core.CoordConversions;

namespace FainCraft.Gameplay.WorldScripts.Core;

public struct VerticalChunkCoord
{
    public int x;
    public int z;

    public VerticalChunkCoord(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static VerticalChunkCoord FromWorldPosition(Vector3 pos)
    {
        return new VerticalChunkCoord
        {
            x = ConvertToChunkFromGlobal(pos.X.FloorToInt()),
            z = ConvertToChunkFromGlobal(pos.Z.FloorToInt())
        };
    }

    public static explicit operator VerticalChunkCoord(ChunkCoord chunkCoord)
    {
        return new VerticalChunkCoord(chunkCoord.X, chunkCoord.Z);
    }

    public static VerticalChunkCoord operator +(VerticalChunkCoord a, VerticalChunkCoord b)
    {
        return new VerticalChunkCoord(a.x + b.x, a.z + b.z);
    }

    public static VerticalChunkCoord operator -(VerticalChunkCoord a, VerticalChunkCoord b)
    {
        return new VerticalChunkCoord(a.x - b.x, a.z - b.z);
    }

    public static VerticalChunkCoord operator *(VerticalChunkCoord a, int b)
    {
        return new VerticalChunkCoord(a.x * b, a.z * b);
    }

    public static VerticalChunkCoord operator *(int b, VerticalChunkCoord a)
    {
        return new VerticalChunkCoord(a.x * b, a.z * b);
    }

    public static bool operator ==(VerticalChunkCoord a, VerticalChunkCoord b)
    {
        return
            a.x == b.x &&
            a.z == b.z;
    }

    public static bool operator !=(VerticalChunkCoord a, VerticalChunkCoord b)
    {
        return
            a.x != b.x ||
            a.z != b.z;
    }

    public override readonly string ToString()
    {
        return $"X: {x} Z:{z}";
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is VerticalChunkCoord coord && this == coord;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }
}
