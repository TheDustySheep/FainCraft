namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering;
public struct VoxelVertex
{
    /// <summary>
    /// Data 1
    /// </summary>
    public uint Data1;

    static readonly uint MASK_XPOS        = 0b_0000_0000_0000_0000_0000_0000_0001_1111;
    static readonly uint MASK_YPOS        = 0b_0000_0000_0000_0000_0000_0011_1110_0000;
    static readonly uint MASK_ZPOS        = 0b_0000_0000_0000_0000_0111_1100_0000_0000;
    static readonly uint MASK_FLIP_FACE   = 0b_0000_0000_0000_0000_1000_0000_0000_0000;
    static readonly uint MASK_CORNER      = 0b_0000_0000_0000_0011_0000_0000_0000_0000;

    static readonly uint MASK_IS_FLUID    = 0b_0000_0000_1000_0000_0000_0000_0000_0000;
    static readonly uint MASK_AO          = 0b_0000_0011_0000_0000_0000_0000_0000_0000;
    static readonly uint MASK_IS_FOLIAGE  = 0b_0000_0100_0000_0000_0000_0000_0000_0000;
    static readonly uint MASK_BIOME_BLEND = 0b_0000_1000_0000_0000_0000_0000_0000_0000;
    static readonly uint MASK_BIOME_INDEX = 0b_1111_0000_0000_0000_0000_0000_0000_0000;

    public uint XPos
    {
        readonly get => Data1 & MASK_XPOS;
        set => Data1 = Data1 & ~MASK_XPOS | value & 31u;
    }

    public uint YPos
    {
        readonly get => (Data1 & MASK_YPOS) >> 5;
        set => Data1 = Data1 & ~MASK_YPOS | (value & 31u) << 5;
    }

    public uint ZPos
    {
        readonly get => (Data1 & MASK_ZPOS) >> 10;
        set => Data1 = Data1 & ~MASK_ZPOS | (value & 31u) << 10;
    }

    public uint FlipFace
    {
        readonly get => (Data1 & MASK_FLIP_FACE) >> 15;
        set => Data1 = Data1 & ~MASK_FLIP_FACE | (value & 1u) << 15;
    }

    public uint Corner
    {
        readonly get => (Data1 & MASK_CORNER) >> 16;
        set => Data1 = Data1 & ~MASK_CORNER | (value & 3u) << 16;
    }

    public uint IsFluid
    {
        readonly get => (Data1 & MASK_IS_FLUID) >> 23;
        set => Data1 = Data1 & ~MASK_IS_FLUID | (value & 1u) << 23;
    }

    public uint AO
    {
        readonly get => (Data1 & MASK_AO) >> 24;
        set => Data1 = Data1 & ~MASK_AO | (value & 3u) << 24;
    }

    public uint Animate_Foliage
    {
        readonly get => (Data1 & MASK_IS_FOLIAGE) >> 26;
        set => Data1 = Data1 & ~MASK_IS_FOLIAGE | (value & 1u) << 26;
    }

    public uint Biome_Blend
    {
        readonly get => (Data1 & MASK_BIOME_BLEND) >> 27;
        set => Data1 = Data1 & ~MASK_BIOME_BLEND | (value & 1u) << 27;
    }

    public uint Biome_Index
    {
        readonly get => (Data1 & MASK_BIOME_INDEX) >> 28;
        set => Data1 = Data1 & ~MASK_BIOME_INDEX | (value & 15u) << 28;
    }

    /// <summary>
    /// Data 2
    /// - Texture Indexes
    /// - Mesh Faces
    /// </summary>
    public uint Data2;

    static readonly uint MASK_TEX_ID  = 0b_0000_0000_0000_0000_1111_1111_1111_1111;
    static readonly uint MASK_MESH_ID = 0b_1111_1111_1111_1111_0000_0000_0000_0000;

    public uint TexIndex
    {
        readonly get => Data2 & MASK_TEX_ID;
        set => Data2 = Data2 & ~MASK_TEX_ID | value & 65535u;
    }

    public uint MeshIndex
    {
        readonly get => Data2 & MASK_MESH_ID >> 16;
        set => Data2 = Data2 & ~MASK_MESH_ID | (value & 65535u) << 16;
    }

    public override string ToString()
    {
        return $"XPos_px:{XPos} YPox_px:{YPos} Z:{ZPos}";
    }
}
