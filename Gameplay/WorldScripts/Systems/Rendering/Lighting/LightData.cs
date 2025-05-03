using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public struct LightData
    {
        const ushort TORCH_MASK = 0b0000011111;
        const ushort SKY_MASK   = 0b1111100000;

        ushort _data;

        public readonly byte TotalLight => byte.Max(Torch, Sky);

        public byte Sky
        {
            readonly get => (byte)((_data & SKY_MASK) >> 5);
            set => _data = (byte)(_data & ~SKY_MASK | (value & 31) << 5);
        }

        public byte Torch
        {
            readonly get => (byte)(_data & TORCH_MASK);
            set => _data = (byte)(_data & ~TORCH_MASK | value & 31);
        }
    }
}
