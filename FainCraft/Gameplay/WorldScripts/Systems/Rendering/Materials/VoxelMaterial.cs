using FainEngine_v2.Extensions;
using FainEngine_v2.Rendering.Materials;
using System.Numerics;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Materials;
internal class VoxelMaterial : Material
{
    public VoxelMaterial(Shader shader, Texture albedo) : base(shader)
    {
        SetTexture(albedo, "albedoTexture");
    }
}
