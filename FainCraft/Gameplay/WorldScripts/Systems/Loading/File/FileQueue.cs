using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using Newtonsoft.Json.Linq;
using Silk.NET.Input;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files
{
    public class FileQueue
    {
        ConcurrentDictionary<RegionCoord, RegionData> _inboxSave = new();

        ConcurrentQueue<RegionCoord>  _inboxLoad = new();
        ConcurrentQueue<RegionPair>  _outboxLoad = new();

        #region Load Requests
        public void LoadRequest(RegionCoord coord)
        {
            _inboxLoad.Enqueue(coord);
        }

        public bool TryDequeueLoadRequest(out RegionCoord coord)
        {
            return _inboxLoad.TryDequeue(out coord);
        }
        #endregion

        #region Load Results
        public void LoadResult(RegionCoord coord, RegionData data)
        {
            _outboxLoad.Enqueue(new RegionPair()
            {
                Coord = coord,
                Data = data
            });
        }

        public bool TryDequeueLoadResult(out RegionCoord coord, out RegionData data)
        {
            bool result = _outboxLoad.TryDequeue(out var pair);
            coord = pair.Coord;
            data = pair.Data;
            return result;
        }
        #endregion

        #region Save Requests
        public void SaveRequest(RegionCoord coord, RegionData data)
        {
            _inboxSave[coord] = data;
        }

        public bool TryDequeueSaveRequest(out RegionCoord coord, out RegionData data)
        {
            foreach (var pair in _inboxSave)
            {
                if (_inboxSave.TryRemove(pair.Key, out var value))
                {
                    coord = pair.Key;
                    data = value;
                    return true;
                }
            }

            coord = default;
            data = default!;
            return false;
        }
        #endregion

        private struct RegionPair
        {
            public RegionCoord Coord;
            public RegionData Data;
        }
    }
}
