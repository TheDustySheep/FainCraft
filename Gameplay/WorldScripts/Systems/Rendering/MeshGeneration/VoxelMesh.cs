using FainEngine_v2.Core;
using FainEngine_v2.Rendering.BoundingShapes;
using FainEngine_v2.Rendering.Meshing;
using System.Numerics;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
internal class VoxelMesh : CustomVertexMesh<VoxelVertex, uint>
{
    public VoxelMesh() : base()
    {
        Bounds = new BoundingBox()
        {
            Min = Vector3.Zero,
            Max = Vector3.One * CHUNK_SIZE
        };
    }

    public VoxelMesh(VoxelVertex[] vertices, uint[] triangles) : base(vertices, triangles)
    {
        Bounds = new BoundingBox()
        {
            Min = Vector3.Zero,
            Max = Vector3.One * CHUNK_SIZE
        };
    }

    protected override Vector3 GetVertexPosition(VoxelVertex vertex)
    {
        return new Vector3(vertex.XPos, vertex.YPos, vertex.ZPos);
    }

    internal void UpdateData(VoxelMeshData data)
    {
        SetTriangles(data.Triangles.ToArray());
        SetVertices(data.Vertices.ToArray());
        Apply();
    }
}
