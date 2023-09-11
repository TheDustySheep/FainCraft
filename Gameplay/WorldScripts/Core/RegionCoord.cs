using Silk.NET.Maths;
using static FainCraft.Gameplay.WorldScripts.Core.CoordConversions;

namespace FainCraft.Gameplay.WorldScripts.Core;
public struct RegionCoord
{
    Vector2D<int> coord;

    public RegionCoord(int x, int z)
    {
        coord = new Vector2D<int>(x, z);
    }

    public RegionCoord(GlobalVoxelCoord globalCoord)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;
        coord.X = chunkCoord.X;
        coord.Y = chunkCoord.Z;
    }

    public RegionCoord(ChunkCoord chunkCoord)
    {
        coord.X = chunkCoord.X;
        coord.Y = chunkCoord.Z;
    }

    public int X { readonly get => coord.X; set => coord.X = value; }
    public int Z { readonly get => coord.Y; set => coord.Y = value; }

    public readonly int Global_Voxel_X => ConvertToGlobalFromChunk(coord.X);
    public readonly int Global_Voxel_Z => ConvertToGlobalFromChunk(coord.Y);

    public static bool operator ==(RegionCoord a, RegionCoord b)
    {
        return a.coord == b.coord;
    }

    public static bool operator !=(RegionCoord a, RegionCoord b)
    {
        return a.coord != b.coord;
    }

    public static RegionCoord operator +(RegionCoord a, RegionCoord b)
    {
        return new RegionCoord(a.X + b.X, a.Z + b.Z);
    }

    public static RegionCoord operator -(RegionCoord a, RegionCoord b)
    {
        return new RegionCoord(a.X - b.X, a.Z - b.Z);
    }

    public override readonly string ToString()
    {
        return coord.ToString();
    }

    public override readonly int GetHashCode()
    {
        return coord.GetHashCode();
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is RegionCoord other &&
               coord.Equals(other.coord);
    }
}
