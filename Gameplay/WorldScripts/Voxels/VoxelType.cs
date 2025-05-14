using Newtonsoft.Json;

namespace FainCraft.Gameplay.WorldScripts.Voxels;
public class VoxelType
{
    [JsonProperty("Name")]               public required string Name;
    [JsonProperty("Draw_Self")]          public bool DrawSelf = true;
    [JsonProperty("EmitsLight")]         public byte Emits_Light = 0;
    [JsonProperty("Fully_Opaque")]       public bool Fully_Opaque = true;
    [JsonProperty("TexIDs")]             public uint[] TexIDs = [0, 0, 0, 0, 0, 0];
    [JsonProperty("Light_Pass_Through")] public bool LightPassThrough;
    [JsonProperty("Physics_Solid")]      public bool Physics_Solid = true;
    [JsonProperty("Is_Fluid")]           public bool Is_Fluid;
    [JsonProperty("Is_Transparent")]     public bool Is_Transparent;
    [JsonProperty("Skip_Draw_Similar")]  public bool Skip_Draw_Similar;
    [JsonProperty("Animate_Foliage")]    public bool Animate_Foliage;
    [JsonProperty("Biome_Blend")]        public bool[] Biome_Blend = [false, false, false, false, false, false];
    [JsonProperty("Custom_Mesh")]        public bool Custom_Mesh;

    public override string ToString()
    {
        return $"Voxel Type: {Name}";
    }
}
