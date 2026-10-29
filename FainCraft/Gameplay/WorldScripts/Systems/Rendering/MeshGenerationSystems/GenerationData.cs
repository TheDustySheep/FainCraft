using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Clusters;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;
internal class GenerationData
{
    public ChunkCoord Coord;
    public ChunkDataCluster ClusterData = new();
    public VoxelMeshData Opaque = new();
    public VoxelMeshData Transparent = new();
}