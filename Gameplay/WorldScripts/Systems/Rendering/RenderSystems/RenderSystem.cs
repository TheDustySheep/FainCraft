using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Collections;
using FainEngine_v2.Rendering;
using FainEngine_v2.Rendering.Materials;
using System.Numerics;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
internal class RenderSystem : IRenderSystem, IDisposable
{
    readonly ObjectPool<VoxelMesh_v2> _meshPool = new();
    readonly Dictionary<ChunkCoord, VoxelMesh_v2> _opaqueMeshes      = new();
    readonly Dictionary<ChunkCoord, VoxelMesh_v2> _transparentMeshes = new();

    readonly Material _opaqueMaterial;
    readonly Material _transparentMaterial;

    public RenderSystem(Material opaqueMaterial, Material transparentMaterial)
    {
        _opaqueMaterial      = opaqueMaterial;
        _transparentMaterial = transparentMaterial;
    }

    ~RenderSystem()
    {
        Dispose();
    }

    public void Draw()
    {
        RegionCoord playerCoord = SharedVariables.PlayerPosition.Value.RegionCoord;
        uint renderDistance = SharedVariables.RenderSettings.Value.RenderRadius;

        foreach ((var coord, var mesh) in _opaqueMeshes)
        {
            if (playerCoord.OctileDistance((RegionCoord)coord) > renderDistance)
                continue;

            var model = Matrix4x4.CreateTranslation(coord.GlobalMin);
            GameGraphics.DrawMesh(mesh, _opaqueMaterial, model);
        }

        foreach ((var coord, var mesh) in _transparentMeshes)
        {
            if (playerCoord.OctileDistance((RegionCoord)coord) > renderDistance)
                continue;

            var model = Matrix4x4.CreateTranslation(coord.GlobalMin);
            GameGraphics.DrawMesh(mesh, _transparentMaterial, model);
        }
    }

    public void UnloadChunk(ChunkCoord coord)
    {
        UnloadOpaque(coord);
        UnloadTransparent(coord);
    }

    private void UnloadOpaque(ChunkCoord coord)
    {
        if (_opaqueMeshes.Remove(coord, out var oldMesh))
        {
            oldMesh.Clear();
            _meshPool.Return(oldMesh);
            DebugVariables.OpaqueMeshCount.Value = _opaqueMeshes.Count;
        }
    }

    private void UnloadTransparent(ChunkCoord coord)
    {
        if (_transparentMeshes.Remove(coord, out var oldMesh))
        {
            oldMesh.Clear();
            _meshPool.Return(oldMesh);
            DebugVariables.TransparentMeshCount.Value = _transparentMeshes.Count;
        }
    }

    public void UpdateChunk(ChunkCoord coord, VoxelMeshData opaque, VoxelMeshData transparent)
    {
        if (opaque.IsEmpty)
        {
            UnloadOpaque(coord);
        }
        else
        {
            if (_opaqueMeshes.TryGetValue(coord, out var mesh))
            {
                mesh.UpdateData(opaque);
            }
            else
            {
                mesh = _meshPool.Request();
                mesh.UpdateData(opaque);
                _opaqueMeshes[coord] = mesh;
                DebugVariables.OpaqueMeshCount.Value = _opaqueMeshes.Count;
            }
        }

        if (transparent.IsEmpty)
        {
            UnloadTransparent(coord);
        }
        else
        {
            if (_transparentMeshes.TryGetValue(coord, out var mesh))
            {
                mesh.UpdateData(transparent);
            }
            else
            {
                mesh = _meshPool.Request();
                mesh.UpdateData(transparent);
                _transparentMeshes[coord] = mesh;
                DebugVariables.OpaqueMeshCount.Value = _transparentMeshes.Count;
            }
        }
    }

    public void Dispose()
    {
        foreach (var mesh in _meshPool.Bag)
            mesh.Dispose();

        foreach (var mesh in _opaqueMeshes.Values)
            mesh.Dispose();

        foreach (var mesh in _transparentMeshes.Values)
            mesh.Dispose();
    }
}
