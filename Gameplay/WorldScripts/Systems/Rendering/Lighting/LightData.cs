using System.Runtime.InteropServices;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LightData
    {
        uint _data;

        public byte Sky
        {
            readonly get => (byte)_data;
            set => _data = value;
        }
    }
}
