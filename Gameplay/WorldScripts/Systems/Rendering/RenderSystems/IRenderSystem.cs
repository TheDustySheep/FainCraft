using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
public interface IRenderSystem
{
    event Action<ChunkCoord>? OnMeshAdded;

    public void Draw();
    public void UnloadChunk(ChunkCoord coord);
    public void UpdateChunk(ChunkCoord coord, VoxelMeshData opaque, VoxelMeshData transparent);
    public void UpdateLighting(RegionCoord coord, LightingRegionData data);
}