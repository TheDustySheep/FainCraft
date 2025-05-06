using FainCraft.Gameplay.WorldScripts.Data;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace FainCraft.Gameplay.WorldScripts.Voxels;
public class VoxelIndexer : IVoxelIndexer
{
    readonly VoxelType[] Voxels;
    readonly Dictionary<VoxelType, uint> TypeToIndex = new();
    readonly Dictionary<string, uint> NameToIndex = new();

    public VoxelDataCache<bool> CacheLightingSolid { get; }

    private VoxelIndexer(VoxelType[] voxels)
    {
        Voxels = voxels;

        UpdateIndex();

        CacheLightingSolid = new VoxelDataCache<bool>(Voxels, v => v.Physics_Solid);

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

    public static class Builder
    {
        private static VoxelType[] LoadFile(string filePath)
        {
            string voxel_text = File.ReadAllText(filePath);
            var voxels = JsonConvert.DeserializeObject<VoxelType[]>(voxel_text);

            if (voxels is not null)
                return voxels;

            Console.WriteLine("Could not load voxels");
            return [];
        }

        public static VoxelIndexer FromFilePath(string filePath = @"Resources\Voxels\Voxels.json")
        {
            return new VoxelIndexer(LoadFile(filePath));
        }

        public static VoxelIndexer FromVoxelTypes(VoxelType[] types)
        {
            return new VoxelIndexer(types);
        }
    }
}
