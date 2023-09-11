using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
public interface IMeshGenerator
{
    public VoxelMeshData GenerateMesh(ChunkDataCluster cluster);
}
