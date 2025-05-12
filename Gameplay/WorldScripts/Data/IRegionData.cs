namespace FainCraft.Gameplay.WorldScripts.Data;

public interface IRegionData
{
    bool GetChunk(int c_y, out ChunkData chunkData);
    bool GetVoxel(int l_x, int i_y, int l_z, out VoxelState voxelState);
    bool SetChunk(int c_y, ChunkData data);
}