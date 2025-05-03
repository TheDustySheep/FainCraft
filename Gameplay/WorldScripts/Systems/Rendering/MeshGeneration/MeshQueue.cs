using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainEngine_v2.Collections;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration
{
    public class MeshQueue
    {
        public int QueueCount => _toGenerate.Count;

        readonly Queue<ChunkDataCluster> _clusterPool = new Queue<ChunkDataCluster>();
        readonly OrderedSet<ChunkCoord> _toGenerate = new();
        readonly ConcurrentQueue<(ChunkCoord coord, ChunkDataCluster cluster)> _buffer = new();
        readonly ConcurrentQueue<(ChunkCoord coord, ChunkDataCluster cluster, VoxelMeshData data)> complete = new();

        public MeshQueue()
        {
            for (int i = 0; i < 128; i++)
            {
                _clusterPool.Enqueue(new ChunkDataCluster());
            }
        }

        #region Requests
        public void EnqueueRequest(ChunkCoord chunkCoord, bool important)
        {
            if (important)
                _toGenerate.AddFirst(chunkCoord);
            else
                _toGenerate.AddLast(chunkCoord);
        }

        public bool TryDequeueRequest(out ChunkCoord coord, out ChunkDataCluster cluster)
        {
            if (_toGenerate.Count == 0)
            {
                coord = default;
                cluster = default!;
                return false;
            }

            if (!_clusterPool.TryDequeue(out cluster!))
            {
                coord = default;
                return false;
            }

            if (!_toGenerate.TryDequeue(out coord))
            {
                _clusterPool.Enqueue(cluster);
                return false;
            }

            return true;
        }
        #endregion
        
        #region Completed
        
        public void EnqueueGeneration(ChunkCoord coord, ChunkDataCluster cluster)
        {
            _buffer.Enqueue((coord, cluster));
        }

        public bool TryDequeueGeneration(out ChunkCoord coord, out ChunkDataCluster cluster)
        {
            if (_buffer.TryDequeue(out var pair))
            {
                coord = pair.coord;
                cluster = pair.cluster;
                return true;
            }

            coord = default;
            cluster = default!;
            return false;
        }

        #endregion

        #region Completed
        public void EnqueueComplete(ChunkCoord coord, ChunkDataCluster cluster, VoxelMeshData meshData)
        {
            complete.Enqueue((coord, cluster, meshData));
        }

        public bool TryDequeueComplete(out ChunkCoord coord, out VoxelMeshData meshData)
        {
            if (complete.TryDequeue(out var pair))
            {
                coord = pair.coord;
                meshData = pair.data;
                _clusterPool.Enqueue(pair.cluster);
                return true;
            }

            coord = default;
            meshData = default!;
            return false;
        }

        #endregion
    }
}
