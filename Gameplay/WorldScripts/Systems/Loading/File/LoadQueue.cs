using FainCraft.Gameplay.WorldScripts.Coords;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Files;

public class LoadQueue
{
    private ConcurrentQueue<RegionCoord> _inBuffer = new();

    public void EnqueueRequest(RegionCoord coord)
    {
        _inBuffer.Enqueue(coord);
    }

    public IEnumerable<(SaveCoord, HashSet<RegionCoord>)> ProcessBuffer()
    {
        while (_inBuffer.TryDequeue(out var rCoord))
        {
            SaveCoord sCoord = (SaveCoord)rCoord;
            if (!_queue.TryGetValue(sCoord, out var set))
            {
                set = new HashSet<RegionCoord>();
                _queue[sCoord] = set;
            }

            set.Add(rCoord);
        }

        while (_queue.Count > 0)
        {
            var kvp = _queue.First();
            _queue.Remove(kvp.Key);
            yield return (kvp.Key, kvp.Value);
        }
    }

    private Dictionary<SaveCoord, HashSet<RegionCoord>> _queue = new();
}
