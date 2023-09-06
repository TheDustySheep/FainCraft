using FainCraft.Gameplay.WorldScripts.Chunking;

namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
internal interface IMeshGenerator
{
    public VoxelMeshData GenerateMesh(ChunkDataCluster cluster);
}
