using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainEngine_v2.Collections;
using FainEngine_v2.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public class LightQueue
    {
        #region In Buffer
        ConcurrentQueue<(RegionCoord, RegionData[])> _inBuffer = new();

        public void EnqueueRequest(RegionCoord regionCoord, RegionData[] regionDatas)
        {
            _inBuffer.Enqueue((regionCoord, regionDatas));
        }
        #endregion

        #region Light Queue
        HashSet<RegionCoord> _lightHash = new();
        List<(RegionCoord, RegionData[])> _lightQueue = new();

        public void ProcessInBuffer(RegionCoord sortPoint)
        {
            while (_inBuffer.TryDequeue(out var pair))
            {
                if (_lightHash.Add(pair.Item1))
                    _lightQueue.Add(pair);
            }

            _lightQueue.SortByDescending(i => i.Item1.ManhattenDistance(sortPoint));
        }

        public bool DequeueRequest(out RegionCoord regionCoord, out RegionData[] regionDatas)
        {
            if (_lightQueue.TryDequeueLast(out var pair))
            {
                regionCoord = pair.Item1;
                regionDatas = pair.Item2;

                _lightHash.Remove(regionCoord);
                return true;
            }

            regionCoord = default!;
            regionDatas = default!;
            return false;
        }
        #endregion

        #region Light Datas
        LimitedObjectPool<LightingRegionData> _dataPool = new(8);
        public bool TryRequestLightingData(out LightingRegionData data)
        {
            return _dataPool.TryRequest(out data);
        }
        public void ReturnLightingData(LightingRegionData data)
        {
            _dataPool.Return(data);
        }
        #endregion

        #region Completed
        ConcurrentQueue<(RegionCoord, LightingRegionData)> _outBuffer = new();
        public void EnqueueComplete(RegionCoord coord, LightingRegionData data)
        {
            _outBuffer.Enqueue((coord, data));
        }

        public bool TryDequeueComplete(out RegionCoord coord, out LightingRegionData data)
        {
            if (_outBuffer.TryDequeue(out var pair))
            {
                coord = pair.Item1;
                data  = pair.Item2;
                return true;
            }

            coord = default!;
            data  = default!;
            return false;
        }
        #endregion
    }
}
