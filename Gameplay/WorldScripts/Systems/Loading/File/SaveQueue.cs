using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files
{
    public class SaveQueue
    {
        private ConcurrentQueue<(RegionCoord, RegionData)> _inBuffer = new();

        public void EnqueueRequest(RegionCoord coord, RegionData data)
        {
            _inBuffer.Enqueue((coord, data));
        }

        public IEnumerable<(SaveCoord, Dictionary<RegionCoord, RegionData>)> ProcessBuffer()
        {
            while (_inBuffer.TryDequeue(out var pair))
            {
                SaveCoord sCoord = (SaveCoord)pair.Item1;
                if(!_queue.TryGetValue(sCoord, out var dict))
                {
                    dict = new Dictionary<RegionCoord, RegionData>();
                    _queue[sCoord] = dict;
                }

                dict[pair.Item1] = pair.Item2;
            }

            while (_queue.Count > 0)
            {
                var kvp = _queue.First();
                _queue.Remove(kvp.Key);
                yield return (kvp.Key, kvp.Value);
            }
        }

        private Dictionary<SaveCoord, Dictionary<RegionCoord, RegionData>> _queue = new();
    }
}
