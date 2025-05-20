using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;

namespace FainCraft.Gameplay.WorldScripts.Data.Regions;

public interface IRegionData
{
    public bool GetChunk(int c_y, out ChunkData chunkData);
    public bool GetVoxel(int l_x, int i_y, int l_z, out VoxelState voxelState);
    public bool SetChunk(int c_y, ChunkData data);
}