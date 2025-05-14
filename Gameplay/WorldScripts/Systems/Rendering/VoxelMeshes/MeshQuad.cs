using Silk.NET.Maths;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;

[StructLayout(LayoutKind.Sequential)]
public struct MeshQuad
{
    public float VertA_X;
    public float VertA_Y;
    public float VertA_Z;

    public float VertB_X;
    public float VertB_Y;
    public float VertB_Z;

    public float VertC_X;
    public float VertC_Y;
    public float VertC_Z;

    public float VertD_X;
    public float VertD_Y;
    public float VertD_Z;

    public float Normal_X;
    public float Normal_Y;
    public float Normal_Z;

    public float UVMin_X;
    public float UVMin_Y;

    public float UVMax_X;
    public float UVMax_Y;

    public uint FaceCoord;

    public Vector3 VertA  { set {  VertA_X = value.X;  VertA_Y = value.Y;  VertA_Z = value.Z; } readonly get => new( VertA_X,  VertA_Y,  VertA_Z); }
    public Vector3 VertB  { set {  VertB_X = value.X;  VertB_Y = value.Y;  VertB_Z = value.Z; } readonly get => new( VertB_X,  VertB_Y,  VertB_Z); }
    public Vector3 VertC  { set {  VertC_X = value.X;  VertC_Y = value.Y;  VertC_Z = value.Z; } readonly get => new( VertC_X,  VertC_Y,  VertC_Z); }
    public Vector3 VertD  { set {  VertD_X = value.X;  VertD_Y = value.Y;  VertD_Z = value.Z; } readonly get => new( VertD_X,  VertD_Y,  VertD_Z); }
    public Vector3 Normal { set { Normal_X = value.X; Normal_Y = value.Y; Normal_Z = value.Z; } readonly get => new(Normal_X, Normal_Y, Normal_Z); }
    public Vector2 UVMin  { set {  UVMin_X = value.X;  UVMin_Y = value.Y; } readonly get => new(UVMin_X, UVMin_Y); }
    public Vector2 UVMax  { set {  UVMax_X = value.X;  UVMax_Y = value.Y; } readonly get => new(UVMax_X, UVMax_Y); }

    public MeshQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 uvMin, Vector2 uvMax)
    {
        VertA = a;
        VertB = b;
        VertC = c;
        VertD = d;

        UVMin = uvMin;
        UVMax = uvMax;

        // Normal calculated from the first triangle
        Normal = Vector3.Normalize(Vector3.Cross(VertB - VertA, VertC - VertA));

        FaceCoord = 0;
    }

    public readonly bool Equals(MeshQuad other) => 
        VertA == other.VertA &&
        VertB == other.VertB &&
        VertC == other.VertC &&
        VertD == other.VertD &&
        Normal == other.Normal &&
        FaceCoord == other.FaceCoord;

    public override readonly bool Equals(object? obj)
    {
        return obj is MeshQuad other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(VertA, VertB, VertC, VertD, Normal, FaceCoord);
    }

    public static bool operator ==(MeshQuad left, MeshQuad right) => left.Equals(right);
    public static bool operator !=(MeshQuad left, MeshQuad right) => !left.Equals(right);
}
