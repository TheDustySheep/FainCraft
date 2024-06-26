﻿namespace FainCraft.Gameplay.WorldScripts.Data;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

internal class RegionData
{
    public readonly ChunkData[] Chunks;

    public RegionData()
    {
        Chunks = new ChunkData[REGION_Y_TOTAL_COUNT];
        for (int y = 0; y < REGION_Y_TOTAL_COUNT; y++)
        {
            Chunks[y] = new ChunkData();
        }
    }

    public RegionData(ChunkData[] _chunks)
    {
        if (_chunks.Length != REGION_Y_TOTAL_COUNT)
            throw new Exception($"Supplied region data array is incorrect. Expected {REGION_Y_TOTAL_COUNT}. Recieved {_chunks.Length}.");
        Chunks = _chunks;
    }

    public ChunkData? GetChunk(int y)
    {
        y += REGION_Y_NEG_COUNT;
        if (y < 0 || y > REGION_Y_TOTAL_COUNT - 1)
            return null;
        return Chunks[y];
    }

    public bool SetChunk(int y, ChunkData data)
    {
        y += REGION_Y_NEG_COUNT;
        if (y < 0 || y > REGION_Y_TOTAL_COUNT - 1)
            return false;

        Chunks[y] = data;
        return true;
    }
}
