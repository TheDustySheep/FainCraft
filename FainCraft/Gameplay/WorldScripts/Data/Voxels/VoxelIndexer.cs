using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;
using System.Runtime.CompilerServices;

namespace FainCraft.Gameplay.WorldScripts.Data.Voxels;
public class VoxelIndexer : IVoxelIndexer
{
    public MeshQuad[] MeshQuads { get; }
    readonly VoxelType[] Voxels;
    readonly Dictionary<VoxelType, uint> TypeToIndex = new();
    readonly Dictionary<string, uint> NameToIndex = new();

    public VoxelDataCache<bool> CacheLightPassThrough { get; }
    public VoxelDataCache<byte> CacheEmitsLight { get; }
    public VoxelDataCache<bool> CustomMesh { get; }

    public VoxelIndexer(VoxelType[] voxels, MeshQuad[] meshQuads)
    {
        MeshQuads = meshQuads;
        Voxels = voxels;

        UpdateIndex();

        CacheLightPassThrough = new VoxelDataCache<bool>(Voxels, v => v.Light_Solid);
        CacheEmitsLight       = new VoxelDataCache<byte>(Voxels, v => v.Light_Emission);
        CustomMesh            = new VoxelDataCache<bool>(Voxels, v => v.Custom_Mesh != null);
    }

    private void UpdateIndex()
    {
        for (uint i = 0; i < Voxels.Length; i++)
        {
            var voxel = Voxels[i];
            TypeToIndex.Add(voxel, i);
            NameToIndex.Add(voxel.Name, i);
        }
    }

    #region Indexing

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VoxelType GetVoxelType(uint index)
    {
        return Voxels[index];
    }

    public VoxelState GetVoxel(string name)
    {
        NameToIndex.TryGetValue(name, out var index);
        return new VoxelState() { Index = index };
    }

    public VoxelState GetVoxel(VoxelType type)
    {
        TypeToIndex.TryGetValue(type, out var index);
        return new VoxelState() { Index = index };
    }

    public uint GetIndex(VoxelType type)
    {
        TypeToIndex.TryGetValue(type, out var index);
        return index;
    }

    public uint GetIndex(string name)
    {
        NameToIndex.TryGetValue(name, out var index);
        return index;
    }

    #endregion
}
