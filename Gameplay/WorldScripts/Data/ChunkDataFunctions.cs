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
}
