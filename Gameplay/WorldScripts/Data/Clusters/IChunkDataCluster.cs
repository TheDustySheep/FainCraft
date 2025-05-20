using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;

namespace FainCraft.Gameplay.WorldScripts.Data;

public interface IChunkDataCluster
{
    public VoxelState GetVoxel(int x, int y, int z);
    public void SetData(ChunkData chunkData);
    public void SetData(ChunkData?[] chunkDatas);
}