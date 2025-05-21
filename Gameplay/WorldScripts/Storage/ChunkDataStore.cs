using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Signals;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Storage
{
    public class ChunkDataStore : IChunkDataStore
    {
        private readonly IEventBus _eventBus;
        private readonly IRegionDataStore _regionStore;

        public ChunkDataStore(IServiceProvider serviceProvider)
        {
            _eventBus    = serviceProvider.Get<IEventBus>();
            _regionStore = serviceProvider.Get<IRegionDataStore>();
        }

        public bool GetChunkData(ChunkCoord coord, out ChunkData data)
        {
            data = null!;
            if (!InBounds(coord))
                return false;

            var rCoord = (RegionCoord)coord;
            if (!_regionStore.GetRegion(rCoord, out var regionData))
                return false;

            return regionData.GetChunk(coord.Y, out data);
        }

        public bool SetChunkData(ChunkCoord coord, ChunkData data)
        {
            if (!InBounds(coord))
                return false;

            var rCoord = (RegionCoord)coord;
            if (!_regionStore.GetRegion(rCoord, out var regionData))
                return false;

            if (!regionData.SetChunk(coord.Y, data))
                return false;

            _eventBus.Publish(new ModifiedChunkDataSignal()
            {
                Coord = coord,
                Data = data,
            });

            return true;
        }

        public bool EditChunkData(ChunkCoord coord, Func<ChunkData, bool> func)
        {
            if (!InBounds(coord))
                return false;

            var rCoord = (RegionCoord)coord;
            if (!_regionStore.GetRegion(rCoord, out var regionData))
                return false;

            if (!regionData.GetChunk(coord.Y, out var data))
                return false;

            if (func.Invoke(data))
            {
                _eventBus.Publish(new ModifiedChunkDataSignal()
                {
                    Coord = coord,
                    Data = data,
                });
            }

            return true;
        }

        private static bool InBounds(ChunkCoord coord) =>
            coord.Y >= -WorldConstants.REGION_NEG_CHUNKS &&
            coord.Y <   WorldConstants.REGION_POS_CHUNKS;
    }
}
