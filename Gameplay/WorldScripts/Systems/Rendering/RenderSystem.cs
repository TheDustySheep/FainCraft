using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
using FainEngine_v2.Core;
using FainEngine_v2.Rendering.Materials;
using System.Numerics;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering;
internal class RenderSystem : IRenderSystem
{
    readonly Queue<VoxelMesh> meshPool = new();
    readonly Dictionary<ChunkCoord, VoxelMesh> chunks = new();
    readonly Material material;

    public RenderSystem(Material material)
    {
        this.material = material;
    }

    public void Draw()
    {
        foreach ((var coord, var mesh) in chunks)
        {
            var model = Matrix4x4.CreateTranslation(coord.GlobalMin);
            GameGraphics.DrawMesh(mesh, material, model);
        }
    }

    public void UpdateChunk(ChunkCoord coord, VoxelMeshData data)
    {
        if (data.IsEmpty)
        {
            if (chunks.Remove(coord, out var oldMesh))
            {
                oldMesh.Clear();
                meshPool.Enqueue(oldMesh);
            }
            return;
        }

        if (chunks.TryGetValue(coord, out var mesh))
        {
            mesh.SetTriangles(data.Triangles);
            mesh.SetVertices(data.Vertices);
            mesh.Apply();
        }
        else
        {
            if (meshPool.Count > 0)
            {
                mesh = meshPool.Dequeue();
                mesh.SetTriangles(data.Triangles);
                mesh.SetVertices(data.Vertices);
            }
            else
            {
                mesh = new VoxelMesh(data.Vertices, data.Triangles);
            }

            mesh.Apply();
            chunks[coord] = mesh;
        }
    }

    /*
    public void UpdateChunk(ChunkCoord coord, VoxelMeshData data)
    {
        if (data.IsEmpty)
        {
            if (chunks.Remove(coord, out var mesh))
            {
                mesh.Clear();
                meshPool.Enqueue(mesh);
            }
        }
        else if (chunks.TryGetValue(coord, out var mesh))
        {
            mesh.SetTriangles(data.Triangles);
            mesh.SetVertices(data.Vertices);
            mesh.Apply();
        }
        else if (meshPool.Count > 0)
        {
            mesh = meshPool.Dequeue();
            mesh.Clear();
            mesh.SetTriangles(data.Triangles);
            mesh.SetVertices(data.Vertices);
            mesh.Apply();
        }
        else 
        { 
            mesh = new VoxelMesh(data.Vertices, data.Triangles);
            mesh.Apply();
            chunks[coord] = mesh;
        }
    }
    */
}
