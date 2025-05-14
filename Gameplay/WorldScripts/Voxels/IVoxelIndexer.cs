using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;

namespace FainCraft.Gameplay.WorldScripts.Voxels;
public interface IVoxelIndexer
{
    public VoxelDataCache<bool> CustomMesh { get; }
    public VoxelDataCache<bool> CacheLightPassThrough { get; }
    public VoxelDataCache<byte> CacheEmitsLight { get; }
    MeshQuad[] MeshQuads { get; }

    public uint GetIndex(string name);
    public uint GetIndex(VoxelType type);
    public VoxelState GetVoxel(string name);
    public VoxelState GetVoxel(VoxelType type);
    public VoxelType GetVoxelType(uint index);
}