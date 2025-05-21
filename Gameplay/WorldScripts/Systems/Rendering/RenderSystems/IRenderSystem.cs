using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
public interface IRenderSystem
{
    event Action<ChunkCoord>? OnMeshAdded;

    public void Draw();
    public bool Exists(RegionCoord coord);
    public void Load(RegionCoord coord);
    public void Unload(RegionCoord coord);
    public void UpdateMesh(ChunkCoord coord, VoxelMeshData opaque, VoxelMeshData transparent);
    public void UpdateLighting(RegionCoord coord, LightingRegionData data);
}