using System.Runtime.InteropServices;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LightData
    {
        private uint _data;

        public byte Sky
        {
            readonly get => (byte)(_data & 0xF);
            set => _data = (byte)((_data & ~0xF) | (value & 0xF));
        }

        public byte Torch
        {
            readonly get => (byte)((_data >> 4) & 0xF);
            set => _data = (byte)((_data & 0xF) | ((value & 0xF) << 4));
        }
    }
}
