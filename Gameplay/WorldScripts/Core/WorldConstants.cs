﻿using System.Runtime.CompilerServices;

namespace FainCraft.Gameplay.WorldScripts.Core;
internal class WorldConstants
{
    public const int CHUNK_SIZE_POWER = 5;

    public const int CHUNK_SIZE_MASK = CHUNK_SIZE - 1;
    public const int CHUNK_SIZE = 1 << CHUNK_SIZE_POWER;
    public const int CHUNK_AREA = CHUNK_SIZE * CHUNK_SIZE;
    public const int CHUNK_VOLUME = CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE;

    public const int REGION_Y_POS_COUNT = 4;
    public const int REGION_Y_NEG_COUNT = 2;
    public const int REGION_Y_TOTAL_COUNT = REGION_Y_POS_COUNT + REGION_Y_NEG_COUNT;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToIndex(int x, int y, int z)
    {
        return z * CHUNK_SIZE * CHUNK_SIZE + y * CHUNK_SIZE + x;
    }
}