using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;

namespace FainCraft.Gameplay.WorldScripts.Storage;

public interface IChunkDataStore
{
    public bool GetChunkData(ChunkCoord coord, out ChunkData data);
    public bool SetChunkData(ChunkCoord coord, ChunkData data);
    public bool EditChunkData(ChunkCoord coord, Func<ChunkData, bool> func);
}
