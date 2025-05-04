using FainEngine_v2.Core;
using FainEngine_v2.Rendering.BoundingShapes;
using FainEngine_v2.Rendering.Meshing;
using Silk.NET.OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering
{
    public sealed class VoxelMesh_v2 : IMesh
    {
        public bool ClipBounds { get; set; } = true;
        public BoundingBox Bounds { get; }

        private uint triangleCount = 0;
        private GL _gl { get; }
        private VertexArrayObject<VoxelVertex, uint> VAO { get; set; }
        private BufferObject<VoxelVertex> VBO { get; set; }
        private BufferObject<uint> EBO { get; set; }

        public VoxelMesh_v2()
        {
            _gl = GameGraphics.GL;
            EBO = new BufferObject<uint>(_gl, BufferTargetARB.ElementArrayBuffer);
            VBO = new BufferObject<VoxelVertex>(_gl, BufferTargetARB.ArrayBuffer);
            VAO = new VertexArrayObject<VoxelVertex, uint>(_gl, VBO, EBO);

            Bounds = new BoundingBox()
            {
                Min = Vector3.Zero,
                Max = Vector3.One * CHUNK_SIZE
            };
        }

        public void UpdateData(VoxelMeshData data)
        {
            Bind();
            triangleCount = (uint)data.Triangles.Count;
            VAO.Bind();
            VBO.SetData(CollectionsMarshal.AsSpan(data.Vertices ));
            EBO.SetData(CollectionsMarshal.AsSpan(data.Triangles));
            VertexAttributes.SetVertexAttributes(_gl, VAO);
        }

        public void Clear()
        {
            triangleCount = 0;
            VAO.Bind();
            VBO.SetData([]);
            EBO.SetData([]);
        }

        public void Bind()
        {
            VAO.Bind();
        }

        public unsafe void Draw()
        {
            if (triangleCount == 0)
                return;

            VAO.Bind();
            //_gl.DrawArrays(PrimitiveType.Triangles, 0, VertexCount);
            _gl.DrawElements(PrimitiveType.Triangles, triangleCount, DrawElementsType.UnsignedInt, (void*)0);
        }

        public void Dispose()
        {
            VAO.Dispose();
            VBO.Dispose();
            EBO.Dispose();
        }
    }
}
