using FainEngine_v2.Core;
using FainEngine_v2.Rendering.Meshing;
//using System.Numerics;
//using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
internal class VoxelMesh : CustomMesh<VoxelVertex, uint>
{
    //private AABB bounds = new(Vector3.Zero, Vector3.One * CHUNK_SIZE);
    //public override AABB Bounds { get => bounds; set => bounds = value; }

    public VoxelMesh(VoxelVertex[] vertices, uint[] triangles) : base(GameGraphics.GL, vertices, triangles)
    {
    }
}
