using System.Runtime.InteropServices;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LightData
    {
        public byte RawData;

        public byte Sky
        {
            readonly get => (byte)(RawData & 0xF);
            set => RawData = (byte)(RawData & 0xF0 | value & 0xF);
        }

        public byte Torch
        {
            readonly get => (byte)(RawData >> 4 & 0xF);
            set => RawData = (byte)(RawData & 0x0F | (value & 0xF) << 4);
        }
    }
}
