using Newtonsoft.Json;

namespace FainCraft.Gameplay.WorldScripts.Voxels;
internal class VoxelType
{
    [JsonProperty("Name")]
    public required string Name { get; set; }

    [JsonProperty("Draw_Self")]
    public required bool DrawSelf { get; set; }

    [JsonProperty("Draw_Solid")]
    public required bool DrawSolid { get; set; }

    [JsonProperty("TexIDs")]
    public required uint[] TexIDs { get; set; }

    [JsonProperty("Physics_Solid")]
    public required bool Physics_Solid { get; set; }
}
