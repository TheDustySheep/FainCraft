using Newtonsoft.Json;

namespace FainCraft.Gameplay.WorldScripts.Voxels;
public class VoxelType
{
    [JsonProperty("Name")]
    public required string Name { get; set; }

    [JsonProperty("Draw_Self")]
    public bool DrawSelf { get; set; } = true;

    [JsonProperty("EmitsLight")]
    public byte Emits_Light { get; set; } = 0;

    [JsonProperty("Fully_Opaque")]
    public bool Fully_Opaque { get; set; } = true;

    [JsonProperty("TexIDs")]
    public uint[] TexIDs { get; set; } = [0, 0, 0, 0, 0, 0];

    [JsonProperty("Light_Pass_Through")]
    public bool LightPassThrough { get; set; }

    [JsonProperty("Physics_Solid")]
    public bool Physics_Solid { get; set; } = true;

    [JsonProperty("Is_Fluid")]
    public bool Is_Fluid { get; set; }

    [JsonProperty("Is_Transparent")]
    public bool Is_Transparent { get; set; }

    [JsonProperty("Skip_Draw_Similar")]
    public bool Skip_Draw_Similar { get; set; }

    [JsonProperty("Animate_Foliage")]
    public bool Animate_Foliage { get; set; }

    [JsonProperty("Biome_Blend")]
    public bool[] Biome_Blend { get; set; } = [false, false, false, false, false, false];



    public override string ToString()
    {
        return $"Voxel Type: {Name}";
    }
}
