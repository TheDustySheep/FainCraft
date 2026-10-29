using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;

namespace FainCraft.Gameplay.WorldScripts.Storage;

public interface IChunkDataStore
{
    public bool GetChunkData(ChunkCoord coord, out ChunkDataRef data);
    public bool SetChunkData(ChunkCoord coord, ChunkDataRef data);
    public bool EditChunkData(ChunkCoord coord, Func<ChunkDataRef, bool> func);
}
