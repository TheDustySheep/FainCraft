﻿namespace FainCraft.Gameplay.WorldScripts.Chunking;

using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

internal class RegionData
{
    ChunkData[] chunks;

    public RegionData()
    {
        chunks = new ChunkData[REGION_Y_TOTAL_COUNT];
    }

    public RegionData(ChunkData[] _chunks)
    {
        if (_chunks.Length != REGION_Y_TOTAL_COUNT)
            throw new Exception($"Supplied region data array is incorrect. Expected {REGION_Y_TOTAL_COUNT}. Recieved {_chunks.Length}.");
        chunks = _chunks;
    }

    public ChunkData? GetChunk(int y)
    {
        y += REGION_Y_NEG_COUNT;
        if (y < 0 || y > REGION_Y_TOTAL_COUNT - 1)
            return null;
        return chunks[y];
    }

    public bool SetChunk(int y, ChunkData data)
    {
        y += REGION_Y_NEG_COUNT;
        if (y < 0 || y > REGION_Y_TOTAL_COUNT - 1)
            return false;

        chunks[y] = data;
        return true;
    }
}
