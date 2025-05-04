using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
using FainCraft.Signals.Gameplay.WorldScripts;
using FainEngine_v2.Collections;
using FainEngine_v2.Core;
using FainEngine_v2.Rendering.Materials;
using System.Diagnostics;
using System.Numerics;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering;
internal class RenderSystem : IRenderSystem, IDisposable
{
    readonly ObjectPool<VoxelMesh_v2> _meshPool = new();
    readonly Dictionary<ChunkCoord, VoxelMesh_v2> _chunks = new();
    readonly Material _material;

    public RenderSystem(Material material)
    {
        _material = material;
    }

    ~RenderSystem()
    {
        Dispose();
    }

    public void Draw()
    {
        foreach ((var coord, var mesh) in _chunks)
        {
            var model = Matrix4x4.CreateTranslation(coord.GlobalMin);
            GameGraphics.DrawMesh(mesh, _material, model);
        }
    }

    public void UnloadChunk(ChunkCoord coord)
    {
        Stopwatch sw = Stopwatch.StartNew();
        if (!_chunks.Remove(coord, out var oldMesh))
            return;

        oldMesh.Clear();
        _meshPool.Return(oldMesh);
        DebugGenerationTimeSignals.LoadedMeshesCountUpdate((uint)_chunks.Count);
        Console.WriteLine($"Removing chunk took {sw.Elapsed.TotalMilliseconds:2F} ms");
    }

    public void UpdateChunk(ChunkCoord coord, VoxelMeshData data)
    {
        Stopwatch sw = Stopwatch.StartNew();
        if (data.IsEmpty)
        {
            UnloadChunk(coord);
            return;
        }

        if (_chunks.TryGetValue(coord, out var mesh))
        {
            mesh.UpdateData(data);
        }
        else
        {
            mesh = _meshPool.Request();
            mesh.UpdateData(data);
            _chunks[coord] = mesh;
            DebugGenerationTimeSignals.LoadedMeshesCountUpdate((uint)_chunks.Count);
        }
        sw.Stop();
        Console.WriteLine($"Updating chunk took {sw.Elapsed.TotalMilliseconds:2F} ms");
    }

    public void Dispose()
    {
        foreach (var mesh in _meshPool.Bag)
            mesh.Dispose();

        foreach (var mesh in _chunks.Values)
            mesh.Dispose();
    }
}
