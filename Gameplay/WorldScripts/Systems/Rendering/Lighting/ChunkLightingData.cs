using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public class ChunkLightingData
    {
        public LightData[] _data = new LightData[34 * 34 * 34];

        public LightData this[uint x, uint y, uint z]
        {
            get => _data[x + z * 34 + y * 1156];
            set => _data[x + z * 34 + y * 1156] = value;
        }
    }
}
