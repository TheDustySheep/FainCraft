using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Voxels;
public interface IVoxelIndexer
{
    public VoxelDataCache<bool> CacheLightPassThrough { get; }

    public uint GetIndex(string name);
    public uint GetIndex(VoxelType type);
    public VoxelState GetVoxel(string name);
    public VoxelState GetVoxel(VoxelType type);
    public VoxelType GetVoxelType(uint index);
}