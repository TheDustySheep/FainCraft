using FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting;
using FainEngine_v2.Rendering;
using FainEngine_v2.Rendering.BoundingShapes;
using FainEngine_v2.Rendering.Meshing;
using Silk.NET.OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;

public sealed class VoxelMesh_v2 : GLObject, IMesh
{
    public bool ClipBounds { get; set; } = true;
    public BoundingBox Bounds { get; }

    private uint triangleCount = 0;

    private VertexArrayObject<VoxelVertex, uint> VAO { get; }
    private BufferObject<VoxelVertex> VBO { get; }
    private BufferObject<uint> EBO { get; }

    private LightingSSBO _lightingSSBO { get; }
    private MeshFaceBuffer _meshFaceBuffer;

    public VoxelMesh_v2(MeshFaceBuffer meshFaceBuffer)
    {
        _meshFaceBuffer = meshFaceBuffer;

        EBO = new BufferObject<uint>(BufferTargetARB.ElementArrayBuffer);
        VBO = new BufferObject<VoxelVertex>(BufferTargetARB.ArrayBuffer);
        VAO = new VertexArrayObject<VoxelVertex, uint>(VBO, EBO);

        _lightingSSBO = new LightingSSBO();

        Bounds = new BoundingBox()
        {
            Min = Vector3.Zero,
            Max = Vector3.One * CHUNK_SIZE
        };

        VertexAttributes.SetVertexAttributes(_GL, VAO);
    }

    public void UpdateData(VoxelMeshData data)
    {
        triangleCount = (uint)data.Triangles.Count;

        VBO.SetData(CollectionsMarshal.AsSpan(data.Vertices ));
        EBO.SetData(CollectionsMarshal.AsSpan(data.Triangles));
    }

    public void Clear()
    {
        triangleCount = 0;

        //_lightingSSBO.Clear();
        _lightingSSBO.FillDefault(); // Max lighting

        VBO.SetData([]);
        EBO.SetData([]);
    }

    public unsafe void Draw()
    {
        if (triangleCount == 0)
            return;

        VAO.Bind();
        _meshFaceBuffer.Bind(0);
        _lightingSSBO.Bind(1);

        _GL.DrawElementsBaseVertex(PrimitiveType.Triangles, triangleCount, DrawElementsType.UnsignedInt, (void*)0, 0);
    }

    internal void UpdateLighting(ReadOnlySpan<LightData> span)
    {
        _lightingSSBO.SetData(span);
    }

    protected override void Release()
    {
        _lightingSSBO.Dispose();
        VAO.Dispose();
        VBO.Dispose();
        EBO.Dispose();
    }
}