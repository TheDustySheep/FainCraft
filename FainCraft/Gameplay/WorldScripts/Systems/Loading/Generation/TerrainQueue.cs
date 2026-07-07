using FainCraft.Gameplay.WorldScripts.Coords;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation
{
    public class TerrainQueue
    {
        readonly ConcurrentQueue<RegionCoord> _inQueue = new();
        readonly HashSet<RegionCoord> _coords = new();

        public int Count => _coords.Count + _inQueue.Count;

        public void Enqueue(RegionCoord coord)
        {
            _inQueue.Enqueue(coord);
        }

        public IEnumerable<RegionCoord> Dequeue(RegionCoord player, int count)
        {
            while (_inQueue.TryDequeue(out var item))
                _coords.Add(item);

            if (_coords.Count == 0)
                yield break;

            var pq = new PriorityQueue<RegionCoord, uint>();
            foreach (var coord in _coords)
                pq.Enqueue(coord, player.ManhattenDistance(coord));

            int i = 0;
            while (i < count && pq.TryDequeue(out var coord, out var _))
            {
                i++;
                _coords.Remove(coord);
                yield return coord;
            }
        }
    }
}
