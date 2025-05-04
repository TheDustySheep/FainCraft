using FainEngine_v2.Extensions;
using FainEngine_v2.Rendering.Materials;
using System.Numerics;

namespace FainCraft.Resources.Shaders.Voxel_Shader;
internal class VoxelMaterialTransparent : Material
{
    public VoxelMaterialTransparent(Shader shader, Texture albedo) : base(shader)
    {
        SetTexture(albedo, "albedoTexture");
        RenderPass = FainEngine_v2.Rendering.RenderPass.Transparent;
    }

    protected override void SetAdditionalUniforms()
    {
        shader.SetUniform("lighting.direction", new Vector3(1, -1, 1).Normalized());
        shader.SetUniform("lighting.ambient", Vector3.One * 0.4f);
        shader.SetUniform("lighting.diffuse", Vector3.One * 0.6f);
        shader.SetUniform("lighting.specular", Vector3.One * 0.6f);
    }
}
