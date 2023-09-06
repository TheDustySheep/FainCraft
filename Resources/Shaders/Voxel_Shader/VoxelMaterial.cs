using FainEngine_v2.Rendering.Materials;
using System.Numerics;
using FainEngine_v2.Extensions;

namespace FainCraft.Resources.Shaders.Voxel_Shader;
internal class VoxelMaterial : Material
{
    public VoxelMaterial(Shader shader, Texture albedo) : base(shader)
    {
        SetTexture(albedo, "albedoTexture");
    }

    protected override void SetAdditionalUniforms()
    {
        shader.SetUniform("lighting.direction", new Vector3(1, -1, 1).Normalize());
        shader.SetUniform("lighting.ambient", Vector3.One * 0.4f);
        shader.SetUniform("lighting.diffuse", Vector3.One * 0.6f);
    }
}
