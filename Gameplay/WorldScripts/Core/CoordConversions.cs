using FainEngine_v2.Extensions;
using System.Runtime.CompilerServices;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Core;

public static class CoordConversions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToGlobalFromChunk(int pos)
    {
        return pos << CHUNK_SIZE_POWER;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToChunkFromGlobal(int pos)
    {
        return pos >> CHUNK_SIZE_POWER;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToLocalFromGlobal(int pos)
    {
        return pos.Mod(CHUNK_SIZE);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToArrayIndex(int x, int y, int z)
    {
        return
            (z & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER * 2 |
            (y & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER |
            (x & CHUNK_SIZE_MASK);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToArrayIndex(uint x, uint y, uint z)
    {
        return
            ((int)z & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER * 2 |
            ((int)y & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER |
            ((int)x & CHUNK_SIZE_MASK);
    }
}
