using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Clusters;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;
internal class GenerationData
{
    public ChunkCoord Coord;
    public IMeshGenerator Generator;
    public IChunkDataCluster ClusterData = new ChunkDataClusterFull();
    public ChunkData[] DataArray = new ChunkData[27];
    public VoxelMeshData Opaque = new();
    public VoxelMeshData Transparent = new();

    public GenerationData(IMeshGenerator generator)
    {
        Generator = generator;
    }
}