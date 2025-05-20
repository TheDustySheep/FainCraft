using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
public interface IMeshGenerator
{
    public void GenerateMesh(IChunkDataCluster cluster, VoxelMeshData opaqueMeshData, VoxelMeshData transparentMeshData);
}
