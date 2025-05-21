using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Signals;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Storage;

public class ChunkClusterDataStore : IChunkClusterDataStore
{
    private readonly IEventBus _eventBus;
    private readonly IChunkDataStore _chunkStore;

    public ChunkClusterDataStore(IServiceProvider serviceProvider)
    {
        _eventBus   = serviceProvider.Get<IEventBus>();
        _chunkStore = serviceProvider.Get<IChunkDataStore>();
    }

    public bool GetChunkClusterData(ChunkCoord coord, Span<ChunkData?> datas)
    {
        int i = 0;
        for (int y = -1; y < 2; y++)
        {
            for (int z = -1; z < 2; z++)
            {
                for (int x = -1; x < 2; x++, i++)
                {
                    var offset = new ChunkCoord(x, y, z);
                    var _coord = offset + coord;

                    _chunkStore.GetChunkData(_coord, out datas[i]);
                }
            }
        }
        return datas[13] != null;
    }
}
