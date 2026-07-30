using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;
using FainEngine_v2.Collections;
using FainEngine_v2.Rendering;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Utils;
using System.Numerics;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
public class DrawingSystem : IDrawingSystem, IDisposable
{
    readonly MeshFaceBuffer _buffer;
    readonly ObjectPoolFactory<VoxelMesh_v2> _meshPool;
    readonly Dictionary<ChunkCoord, Chunk> _chunks = new();

    readonly Material _opaqueMaterial;
    readonly Material _transparentMaterial;

    readonly IGameGraphics _graphics;

    public DrawingSystem(Material opaqueMaterial, Material transparentMaterial, IVoxelIndexer indexer)
    {
        _graphics = DependencyInjector.Resolve<IGameGraphics>();

        _buffer   = new MeshFaceBuffer(indexer.MeshQuads);
        _meshPool = new(() => new VoxelMesh_v2(_buffer));

        _opaqueMaterial      = opaqueMaterial;
        _transparentMaterial = transparentMaterial;
    }

    ~DrawingSystem() => Dispose();

    public void Draw()
    {
        RegionCoord playerCoord = SharedVariables.PlayerPosition.Value.RegionCoord;
        uint renderDistance = SharedVariables.RenderSettings.Value.RenderRadius;

        foreach ((var coord, var chunk) in _chunks)
        {
            if (chunk.OpaqueMesh == null && chunk.TransparentMesh == null)
                continue;

            if (playerCoord.OctileDistance((RegionCoord)coord) > renderDistance)
                continue;

            var model = Matrix4x4.CreateTranslation(coord.GlobalMin);

            if (chunk.OpaqueMesh != null)
                _graphics.DrawMesh(chunk.OpaqueMesh, _opaqueMaterial, model);

            if (chunk.TransparentMesh != null)
                _graphics.DrawMesh(chunk.TransparentMesh, _transparentMaterial, model);
        }
    }

    public void Load(RegionCoord coord)
    {
        for (int y = -REGION_NEG_CHUNKS; y < REGION_POS_CHUNKS; y++)
        {
            _chunks.TryAdd(new ChunkCoord(coord, y), new Chunk());
        }
    }

    public void Unload(RegionCoord coord)
    {
        for (int y = -REGION_NEG_CHUNKS; y < REGION_POS_CHUNKS; y++)
        {
            if (!_chunks.Remove(new ChunkCoord(coord, y), out var chunk))
                continue;

            if (chunk.OpaqueMesh != null)
            {
                _meshPool.Return(chunk.OpaqueMesh);
                DebugVariables.OpaqueMeshCount.Value -= 1;
            }

            if (chunk.TransparentMesh != null)
            {
                _meshPool.Return(chunk.TransparentMesh);
                DebugVariables.TransparentMeshCount.Value -= 1;
            }
        }
    }

    public void UpdateMesh(ChunkCoord coord, VoxelMeshData opaque, VoxelMeshData transparent)
    {
        if (!_chunks.TryGetValue(coord, out var chunk))
            return;

        if (opaque.IsEmpty)
        {
            if (chunk.OpaqueMesh != null)
            {
                _meshPool.Return(chunk.OpaqueMesh);
                DebugVariables.OpaqueMeshCount.Value -= 1;
                chunk.OpaqueMesh = null;
            }
        }
        else
        {
            if (chunk.OpaqueMesh == null)
            {
                chunk.OpaqueMesh = _meshPool.Request();
                DebugVariables.OpaqueMeshCount.Value += 1;
            }

            chunk.OpaqueMesh.UpdateData(opaque);
        }

        if (transparent.IsEmpty)
        {
            if (chunk.TransparentMesh != null)
            {
                _meshPool.Return(chunk.TransparentMesh);
                DebugVariables.TransparentMeshCount.Value -= 1;
                chunk.TransparentMesh = null;
            }
        }
        else
        {
            if (chunk.TransparentMesh == null)
            {
                chunk.TransparentMesh = _meshPool.Request();
                DebugVariables.TransparentMeshCount.Value += 1;
            }

            chunk.TransparentMesh.UpdateData(transparent);
        }
    }

    public void UpdateLighting(RegionCoord rCoord, LightingRegionData data)
    {
        for (int c_y = -REGION_NEG_CHUNKS; c_y < REGION_POS_CHUNKS; c_y++)
        {
            var cCoord = new ChunkCoord(rCoord, c_y);
            if (!_chunks.TryGetValue(cCoord, out var chunk))
                continue;

            if (chunk.TransparentMesh == null)
                continue;

            ReadOnlySpan<LightData> span = data.GetSlice(c_y);
            chunk.TransparentMesh.UpdateLighting(span);
        }
    }

    public void Dispose()
    {
        foreach (var mesh in _meshPool.Bag)
            mesh.Dispose();

        foreach (var chunk in _chunks.Values)
        {
            chunk.OpaqueMesh?.Dispose();
            chunk.TransparentMesh?.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    private class Chunk
    {
        public VoxelMesh_v2? OpaqueMesh;
        public VoxelMesh_v2? TransparentMesh;
    }
}
