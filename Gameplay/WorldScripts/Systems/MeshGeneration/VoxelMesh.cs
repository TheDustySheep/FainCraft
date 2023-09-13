using FainEngine_v2.Core;
using FainEngine_v2.Rendering.BoundingShapes;
using FainEngine_v2.Rendering.Meshing;
using System.Numerics;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

//using System.Numerics;
//using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
internal class VoxelMesh : CustomVertexMesh<VoxelVertex, uint>
{
    //private AABB bounds = new(Vector3.Zero, Vector3.One * CHUNK_SIZE);
    //public override AABB Bounds { get => bounds; set => bounds = value; }

    public VoxelMesh(VoxelVertex[] vertices, uint[] triangles) : base(GameGraphics.GL, vertices, triangles)
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
}
