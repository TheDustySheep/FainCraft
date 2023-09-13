using Silk.NET.Maths;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Core;
public struct RegionCoord
{
    Vector2D<int> coord;

    public RegionCoord(int x, int z)
    {
        coord = new Vector2D<int>(x, z);
    }

    public int X { readonly get => coord.X; set => coord.X = value; }
    public int Z { readonly get => coord.Y; set => coord.Y = value; }

    public readonly int Global_Voxel_X => ConvertToGlobalFromChunk(coord.X);
    public readonly int Global_Voxel_Z => ConvertToGlobalFromChunk(coord.Y);

    #region Conversions
    public static explicit operator GlobalVoxelCoord(RegionCoord chunkCoord)
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
    #endregion

    #region Overrides
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
    #endregion
}
