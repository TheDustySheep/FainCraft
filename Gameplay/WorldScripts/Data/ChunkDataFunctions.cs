using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data;

public static class ChunkDataFunctions
{
    public static void GetPaddedChunk1<T>(T[][] src, T[] dst) where T : struct
    {
        int paddedSize = CHUNK_SIZE + 2;

        T Fetch(T[] src, int x, int y, int z)
        {
            if (src == null) return default;
            return src[x + z * CHUNK_SIZE + y * CHUNK_SIZE * CHUNK_SIZE];
        }

        for (int y = 0; y < paddedSize; y++)
        {
            for (int z = 0; z < paddedSize; z++)
            {
                for (int x = 0; x < paddedSize; x++)
                {
                    int sx = x - 1;
                    int sy = y - 1;
                    int sz = z - 1;

                    int idx;
                    T value;

                    // Inside center chunk bounds
                    if (sx >= 0 && sx < CHUNK_SIZE && sy >= 0 && sy < CHUNK_SIZE && sz >= 0 && sz < CHUNK_SIZE)
                    {
                        idx = 13; // center
                        value = Fetch(src[idx], sx, sy, sz);
                    }
                    else
                    {
                        int dx = sx < 0 ? -1 : (sx >= CHUNK_SIZE ? 1 : 0);
                        int dy = sy < 0 ? -1 : (sy >= CHUNK_SIZE ? 1 : 0);
                        int dz = sz < 0 ? -1 : (sz >= CHUNK_SIZE ? 1 : 0);

                        idx = (dz + 1) * 9 + (dy + 1) * 3 + (dx + 1);

                        int cx = sx < 0 ? sx + CHUNK_SIZE : (sx >= CHUNK_SIZE ? sx - CHUNK_SIZE : sx);
                        int cy = sy < 0 ? sy + CHUNK_SIZE : (sy >= CHUNK_SIZE ? sy - CHUNK_SIZE : sy);
                        int cz = sz < 0 ? sz + CHUNK_SIZE : (sz >= CHUNK_SIZE ? sz - CHUNK_SIZE : sz);

                        value = Fetch(src[idx], cx, cy, cz);
                    }

                    dst[x + z * paddedSize + y * paddedSize * paddedSize] = value;
                }
            }
        }
    }

    private const int PADDED_SIZE   = CHUNK_SIZE + 2;
    private const int PADDED_VOLUME = PADDED_SIZE * PADDED_SIZE * PADDED_SIZE;

    public static void GetPaddedChunk2<T>(T[][] srcChunks, Span<T> dst) where T : struct
    {
        if (srcChunks == null || srcChunks.Length != 27)
            throw new ArgumentException("Must provide exactly 27 srcChunks");
        if (dst.Length != PADDED_VOLUME)
            throw new ArgumentException($"Destination must be length {PADDED_VOLUME}");

        // Precache spans
        var spans = srcChunks;
        var dstSpan = dst;
        int di = 0;

        for (int y = 0; y < PADDED_SIZE; y++)
        {
            int sy = y - 1;
            uint uSy = (uint)sy;
            for (int z = 0; z < PADDED_SIZE; z++)
            {
                int sz = z - 1;
                uint uSz = (uint)sz;
                for (int x = 0; x < PADDED_SIZE; x++)
                {
                    int sx = x - 1;
                    uint uSx = (uint)sx;

                    // Bound check
                    bool inCenter = uSx < CHUNK_SIZE && uSy < CHUNK_SIZE && uSz < CHUNK_SIZE;
                    int idx = inCenter
                              ? 13
                              : ((sz < 0 ? -1 : sz >= CHUNK_SIZE ? 1 : 0) + 1) * 9
                                + ((sy < 0 ? -1 : sy >= CHUNK_SIZE ? 1 : 0) + 1) * 3
                                + ((sx < 0 ? -1 : sx >= CHUNK_SIZE ? 1 : 0) + 1);

                    int cx = inCenter ? sx : (sx < 0 ? sx + CHUNK_SIZE : sx >= CHUNK_SIZE ? sx - CHUNK_SIZE : sx);
                    int cy = inCenter ? sy : (sy < 0 ? sy + CHUNK_SIZE : sy >= CHUNK_SIZE ? sy - CHUNK_SIZE : sy);
                    int cz = inCenter ? sz : (sz < 0 ? sz + CHUNK_SIZE : sz >= CHUNK_SIZE ? sz - CHUNK_SIZE : sz);

                    var span = spans[idx];
                    T value = span.Length == 0
                        ? default
                        : span[cx + cz * CHUNK_SIZE + cy * CHUNK_AREA];

                    dstSpan[di++] = value;
                }
            }
        }
    }

    public static void GetPaddedChunk3<T>(T[][] srcChunks, Span<T> dst)
    {
        Span<T> span00 = srcChunks[00].AsSpan();
        Span<T> span01 = srcChunks[01].AsSpan();
        Span<T> span02 = srcChunks[02].AsSpan();
        Span<T> span03 = srcChunks[03].AsSpan();
        Span<T> span04 = srcChunks[04].AsSpan();
        Span<T> span05 = srcChunks[05].AsSpan();
        Span<T> span06 = srcChunks[06].AsSpan();
        Span<T> span07 = srcChunks[07].AsSpan();
        Span<T> span08 = srcChunks[08].AsSpan();
        Span<T> span09 = srcChunks[09].AsSpan();
        Span<T> span10 = srcChunks[10].AsSpan();
        Span<T> span11 = srcChunks[11].AsSpan();
        Span<T> span12 = srcChunks[12].AsSpan();
        Span<T> span13 = srcChunks[13].AsSpan();
        Span<T> span14 = srcChunks[14].AsSpan();
        Span<T> span15 = srcChunks[15].AsSpan();
        Span<T> span16 = srcChunks[16].AsSpan();
        Span<T> span17 = srcChunks[17].AsSpan();
        Span<T> span18 = srcChunks[18].AsSpan();
        Span<T> span19 = srcChunks[19].AsSpan();
        Span<T> span20 = srcChunks[20].AsSpan();
        Span<T> span21 = srcChunks[21].AsSpan();
        Span<T> span22 = srcChunks[22].AsSpan();
        Span<T> span23 = srcChunks[23].AsSpan();
        Span<T> span24 = srcChunks[24].AsSpan();
        Span<T> span25 = srcChunks[25].AsSpan();
        Span<T> span26 = srcChunks[26].AsSpan();

        int di = 0;
        for (int y = 0; y < PADDED_SIZE; y++)
        {
            int sy = y - 1;
            uint uSy = (uint)sy;
            for (int z = 0; z < PADDED_SIZE; z++)
            {
                int sz = z - 1;
                uint uSz = (uint)sz;
                for (int x = 0; x < PADDED_SIZE; x++)
                {
                    int sx = x - 1;
                    uint uSx = (uint)sx;

                    bool inCenter = uSx < CHUNK_SIZE && uSy < CHUNK_SIZE && uSz < CHUNK_SIZE;

                    int dx = sx < 0 ? -1 : (sx >= CHUNK_SIZE ? 1 : 0);
                    int dy = sy < 0 ? -1 : (sy >= CHUNK_SIZE ? 1 : 0);
                    int dz = sz < 0 ? -1 : (sz >= CHUNK_SIZE ? 1 : 0);
                    int idx = (dz + 1) * 9 + (dy + 1) * 3 + (dx + 1);

                    //int idx = inCenter
                    //          ? 13
                    //          : ((sz < 0 ? -1 : sz >= CHUNK_SIZE ? 1 : 0) + 1) * 9
                    //            + ((sy < 0 ? -1 : sy >= CHUNK_SIZE ? 1 : 0) + 1) * 3
                    //            + ((sx < 0 ? -1 : sx >= CHUNK_SIZE ? 1 : 0) + 1);

                    int cx = inCenter ? sx : (sx < 0 ? sx + CHUNK_SIZE : sx >= CHUNK_SIZE ? sx - CHUNK_SIZE : sx);
                    int cy = inCenter ? sy : (sy < 0 ? sy + CHUNK_SIZE : sy >= CHUNK_SIZE ? sy - CHUNK_SIZE : sy);
                    int cz = inCenter ? sz : (sz < 0 ? sz + CHUNK_SIZE : sz >= CHUNK_SIZE ? sz - CHUNK_SIZE : sz);

                    Span<T> currentSpan = idx switch
                    {
                        0 => span00,
                        1 => span01,
                        2 => span02,
                        3 => span03,
                        4 => span04,
                        5 => span05,
                        6 => span06,
                        7 => span07,
                        8 => span08,
                        9 => span09,
                        10 => span10,
                        11 => span11,
                        12 => span12,
                        13 => span13,
                        14 => span14,
                        15 => span15,
                        16 => span16,
                        17 => span17,
                        18 => span18,
                        19 => span19,
                        20 => span20,
                        21 => span21,
                        22 => span22,
                        23 => span23,
                        24 => span24,
                        25 => span25,
                        26 => span26,
                        _ => throw new NotImplementedException()
                    };

                    dst[di++] = currentSpan[cx + cz * CHUNK_SIZE + cy * CHUNK_AREA];
                }
            }
        }
    }

    public static void GetPaddedChunk4<T>(T[][] src, Span<T> dst) where T : struct
    {
        int paddedSize = CHUNK_SIZE + 2;

        static T Fetch(T[] src, int x, int y, int z)
        {
            return src[x + z * CHUNK_SIZE + y * CHUNK_SIZE * CHUNK_SIZE];
        }

        for (int x = 0; x < paddedSize; x++)
        {
            for (int z = 0; z < paddedSize; z++)
            {
                for (int y = 0; y < paddedSize; y++)
                {
                    int sx = x - 1;
                    int sy = y - 1;
                    int sz = z - 1;

                    int idx;
                    T value;

                    // Inside center chunk bounds
                    if (sx >= 0 && sx < CHUNK_SIZE && sy >= 0 && sy < CHUNK_SIZE && sz >= 0 && sz < CHUNK_SIZE)
                    {
                        idx = 13; // center
                        value = Fetch(src[idx], sx, sy, sz);
                    }
                    else
                    {
                        int dx = sx < 0 ? -1 : (sx >= CHUNK_SIZE ? 1 : 0);
                        int dy = sy < 0 ? -1 : (sy >= CHUNK_SIZE ? 1 : 0);
                        int dz = sz < 0 ? -1 : (sz >= CHUNK_SIZE ? 1 : 0);

                        idx = (dz + 1) * 9 + (dy + 1) * 3 + (dx + 1);

                        int cx = sx < 0 ? sx + CHUNK_SIZE : (sx >= CHUNK_SIZE ? sx - CHUNK_SIZE : sx);
                        int cy = sy < 0 ? sy + CHUNK_SIZE : (sy >= CHUNK_SIZE ? sy - CHUNK_SIZE : sy);
                        int cz = sz < 0 ? sz + CHUNK_SIZE : (sz >= CHUNK_SIZE ? sz - CHUNK_SIZE : sz);

                        value = Fetch(src[idx], cx, cy, cz);
                    }

                    dst[x + z * paddedSize + y * paddedSize * paddedSize] = value;
                }
            }
        }
    }
}
