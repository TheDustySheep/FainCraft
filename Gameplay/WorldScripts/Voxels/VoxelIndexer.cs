using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace FainCraft.Gameplay.WorldScripts.Voxels;
internal class VoxelIndexer
{
    VoxelType[] Voxels = Array.Empty<VoxelType>();

    Dictionary<VoxelType, uint> TypeToIndex = new();
    Dictionary<string, uint> NameToIndex = new();

    public void LoadVoxels()
    {
        string voxel_text = File.ReadAllText(@"Resources\Voxels\Voxels.json");
        var voxels = JsonConvert.DeserializeObject<VoxelType[]>(voxel_text);
        if (voxels is null)
        {
            Console.WriteLine("Could not load voxels");
            return;
        }

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
