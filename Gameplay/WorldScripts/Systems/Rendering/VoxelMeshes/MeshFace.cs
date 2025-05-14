using Silk.NET.Maths;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;

[StructLayout(LayoutKind.Sequential)]
public struct MeshFace
{
    public required Vector3 VertA; float _padA; //fkn opengl padding >:(
    public required Vector3 VertB; float _padB; //fkn opengl padding >:(
    public required Vector3 VertC; float _padC; //fkn opengl padding >:(
    public required Vector3 VertD; float _padD; //fkn opengl padding >:(

    public required Vector3 Normal;
    public required uint FaceCoord;
}
