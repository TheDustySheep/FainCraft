using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;
using Newtonsoft.Json;

namespace FainCraft.Gameplay.WorldScripts.Data.Voxels;
public class VoxelType
{
    public required string Name;
    
    // Drawing
    public required bool Draw_Self;
    public required bool Draw_Opaque;
    public required bool Draw_Transparent;
    public required bool Draw_Similar;

    // Foliage
    public required bool   Foliage_Animate;
    public required bool[] Foliage_Biome_Blend;

    // Physics
    public required bool Physics_Solid;

    // Fluid
    public required bool Is_Fluid;

    // Textures
    public required uint[] TexIDs;

    // Meshing
    public required CustomVoxel? Custom_Mesh;
    
    // Lighting
    public byte Light_Emission;
    public bool Light_Solid;

    public override string ToString()
    {
        return $"Voxel Type: {Name}";
    }
}
