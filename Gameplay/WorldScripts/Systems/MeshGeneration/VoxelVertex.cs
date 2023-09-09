namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
internal struct VoxelVertex
{
    /// <summary>
    /// Data 1
    /// </summary>
    public uint Data1;

    static readonly uint MASK_XPOS     = 0b_0000_0000_0000_0000_0000_0000_0011_1111;
    static readonly uint MASK_YPOS     = 0b_0000_0000_0000_0000_0000_1111_1100_0000;
    static readonly uint MASK_ZPOS     = 0b_0000_0000_0000_0011_1111_0000_0000_0000;
    static readonly uint MASK_CORNER   = 0b_0000_0000_0000_1100_0000_0000_0000_0000;
    static readonly uint MASK_NORMAL   = 0b_0000_0000_0111_0000_0000_0000_0000_0000;
    static readonly uint MASK_IS_FLUID = 0b_0000_0000_1000_0000_0000_0000_0000_0000;

    public uint XPos
    {
        readonly get => Data1 & MASK_XPOS;
        set => Data1 = Data1 & ~MASK_XPOS | value & 63u;
    }

    public uint YPos
    {
        readonly get => (Data1 & MASK_YPOS) >> 6;
        set => Data1 = Data1 & ~MASK_YPOS | (value & 63u) << 6;
    }

    public uint ZPos
    {
        readonly get => (Data1 & MASK_ZPOS) >> 12;
        set => Data1 = Data1 & ~MASK_ZPOS | (value & 63u) << 12;
    }

    public uint Corner
    {
        readonly get => (Data1 & MASK_CORNER) >> 18;
        set => Data1 = Data1 & ~MASK_CORNER | (value & 3u) << 18;
    }

    public uint Normal
    {
        readonly get => (Data1 & MASK_NORMAL) >> 20;
        set => Data1 = Data1 & ~MASK_NORMAL | (value & 7u) << 20;
    }

    public uint SurfaceFluid
    {
        readonly get => (Data1 & MASK_IS_FLUID) >> 23;
        set => Data1 = Data1 & ~MASK_IS_FLUID | (value & 1u) << 23;
    }

    /// <summary>
    /// Data 2
    /// </summary>
    public uint Data2;

    static readonly uint MASK_TEXID = 0b_0000_0000_0000_0000_1111_1111_1111_1111;

    public uint TexIndex
    {
        readonly get => Data2 & MASK_TEXID;
        set => Data2 = Data2 & ~MASK_TEXID | value & 65535u;
    }

    public override string ToString()
    {
        return $"X:{XPos} Y:{YPos} Z:{ZPos}";
    }
}
