using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;

namespace FainCraft.Gameplay.WorldScripts.Storage;

public interface IChunkClusterDataStore
{
    public bool GetChunkClusterData(ChunkCoord coord, Span<ChunkData?> datas);
    public Task<bool> GetChunkClusterDataAsync(ChunkCoord coord, ChunkData?[] datas, CancellationToken token);
}
