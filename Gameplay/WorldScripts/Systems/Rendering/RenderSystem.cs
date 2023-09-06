using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
using FainEngine_v2.Core;
using FainEngine_v2.Rendering.Materials;
using System.Numerics;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering;
internal class RenderSystem : IRenderSystem
{
    Dictionary<ChunkCoord, VoxelMesh> chunks = new();
    Material material;

    public RenderSystem(Material material)
    {
        this.material = material;
    }

    public void Draw()
    {
        foreach ((var coord, var mesh) in chunks)
        {
            var model = Matrix4x4.CreateTranslation(coord.GlobalCorner);
            GameGraphics.DrawMesh(mesh, material, model);
        }
    }

    public void UpdateChunk(ChunkCoord coord, VoxelMeshData data)
    {
        if (chunks.TryGetValue(coord, out var mesh))
        {
            mesh.SetTriangles(data.Triangles);
            mesh.SetVertices(data.Vertices);
            mesh.Apply();
            //mesh.RecalculateBounds();
        }
        else
        {
            mesh = new VoxelMesh(data.Vertices, data.Triangles);
            //mesh.RecalculateBounds();
            chunks[coord] = mesh;
        }
    }
}
