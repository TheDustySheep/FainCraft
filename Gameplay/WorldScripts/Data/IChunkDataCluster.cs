namespace FainCraft.Gameplay.WorldScripts.Data;

public interface IChunkDataCluster
{
    public VoxelState GetVoxel(int x, int y, int z);
    public void SetData(ChunkData chunkData);
    public void SetData(ChunkData?[] chunkDatas);
}