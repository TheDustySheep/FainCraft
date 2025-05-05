using FainEngine_v2.Extensions;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.RenderObjects;
using System.Numerics;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Materials;
internal class VoxelMaterialTransparent : Material
{
    bool hasSetRT = false;
    public VoxelMaterialTransparent(Shader shader, Texture albedo) : base(shader)
    {
        SetTexture(albedo, "albedoTexture");
        RenderPass = FainEngine_v2.Rendering.RenderPass.Transparent;
    }

    public override void SetRenderTexture(RenderTexture texture)
    {
        if (!hasSetRT)
        {
            SetTexture(texture.DepthTexture, "depthTexture");
            hasSetRT = true;
        }
    }
}
