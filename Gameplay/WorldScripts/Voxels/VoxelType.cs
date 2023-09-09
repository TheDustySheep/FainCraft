using Newtonsoft.Json;

namespace FainCraft.Gameplay.WorldScripts.Voxels;
internal class VoxelType
{
    [JsonProperty("Name")]
    public required string Name { get; set; }

    [JsonProperty("Draw_Self")]
    public required bool DrawSelf { get; set; }

    [JsonProperty("See_Through")]
    public required bool SeeThrough { get; set; }

    [JsonProperty("TexIDs")]
    public required uint[] TexIDs { get; set; }

    [JsonProperty("Physics_Solid")]
    public required bool Physics_Solid { get; set; }

    [JsonProperty("Is_Fluid")]
    public required bool Is_Fluid { get; set; }

    [JsonProperty("Is_Transparent")]
    public required bool Is_Transparent { get; set; }

    [JsonProperty("Skip_Draw_Similar")]
    public required bool Skip_Draw_Similar { get; set; }
}
