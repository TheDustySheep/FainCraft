using FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting;
using FainEngine_v2.Rendering.Meshing;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes
{
    public class LightingSSBO : ShaderStorageBufferObject<LightData>
    {
        public LightingSSBO() : base(PADDED_CHUNK_VOLUME)
        {
            FillDefault();
        }

        public void FillDefault()
        {
            var light = new LightData() { Sky = 15 };
            FillByte(light.RawData);
        }
    }
}
