using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering;
internal interface IRenderSystem
{
    void Draw();
    void UpdateChunk(ChunkCoord coord, VoxelMeshData data);
}