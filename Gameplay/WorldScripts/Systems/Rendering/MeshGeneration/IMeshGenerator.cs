﻿using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
public interface IMeshGenerator
{
    public void GenerateMesh(IChunkDataCluster cluster, VoxelMeshData opaqueMeshData, VoxelMeshData transparentMeshData);
}
