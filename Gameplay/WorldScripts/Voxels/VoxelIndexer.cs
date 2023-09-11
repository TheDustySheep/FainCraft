using FainCraft.Gameplay.WorldScripts.Data;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace FainCraft.Gameplay.WorldScripts.Voxels;
public class VoxelIndexer
{
    VoxelType[] Voxels = Array.Empty<VoxelType>();

    readonly Dictionary<VoxelType, uint> TypeToIndex = new();
    readonly Dictionary<string, uint> NameToIndex = new();

    public void LoadVoxels()
    {
        string voxel_text = File.ReadAllText(@"Resources\Voxels\Voxels.json");
        var voxels = JsonConvert.DeserializeObject<VoxelType[]>(voxel_text);
        if (voxels is null)
        {
            Console.WriteLine("Could not load voxels");
            return;
        }

        LoadVoxels(voxels);
    }

    public void LoadVoxels(VoxelType[] voxels)
    {
        Voxels = voxels;

        for (uint i = 0; i < Voxels.Length; i++)
        {
            var voxel = Voxels[i];
            TypeToIndex.Add(voxel, i);
            NameToIndex.Add(voxel.Name, i);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VoxelType GetVoxelType(uint index)
    {
        return Voxels[index];
    }
    public VoxelData GetVoxel(string name)
    {
        NameToIndex.TryGetValue(name, out var index);
        return new VoxelData() { Index = index };
    }

    public VoxelData GetVoxel(VoxelType type)
    {
        TypeToIndex.TryGetValue(type, out var index);
        return new VoxelData() { Index = index };
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
}
