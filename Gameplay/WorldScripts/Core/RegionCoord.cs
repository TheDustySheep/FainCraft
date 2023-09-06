using Silk.NET.Maths;

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
