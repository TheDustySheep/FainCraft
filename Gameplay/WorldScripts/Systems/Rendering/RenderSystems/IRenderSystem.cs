using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
internal interface IRenderSystem
{
    void Draw();
    void UnloadChunk(ChunkCoord coord);
    void UpdateChunk(ChunkCoord coord, VoxelMeshData data);
}