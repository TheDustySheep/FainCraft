using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainEngine_v2.Collections;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration
{
    public class MeshQueue
    {
        public int QueueCount => _toGenerate.Count;

        readonly ObjectPool<ChunkDataCluster> _clusterPool  = new() { MaxItems = 128 };
        readonly ObjectPool<VoxelMeshData>    _meshDataPool = new() { MaxItems = 128 };

        readonly OrderedSet<ChunkCoord> _toGenerate = new();
        readonly ConcurrentQueue<(ChunkCoord coord, ChunkDataCluster cluster)> _buffer = new();
        readonly ConcurrentQueue<(ChunkCoord coord, ChunkDataCluster cluster, VoxelMeshData opaque, VoxelMeshData transparent)> complete = new();

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
            if (!_toGenerate.TryDequeue(out coord))
            {
                coord = default;
                cluster = default!;
                return false;
            }

            cluster = _clusterPool.Request();
            return true;
        }
        #endregion
        
        #region Completed
        
        public void EnqueueGeneration(ChunkCoord coord, ChunkDataCluster cluster)
        {
            _buffer.Enqueue((coord, cluster));
        }

        public bool TryDequeueGeneration(
            out ChunkCoord coord, 
            out ChunkDataCluster cluster, 
            out VoxelMeshData meshData1,
            out VoxelMeshData meshData2
        )
        {
            if (_buffer.TryDequeue(out var pair))
            {
                coord     = pair.coord;
                cluster   = pair.cluster;
                meshData1 = _meshDataPool.Request();
                meshData2 = _meshDataPool.Request();

                return true;
            }

            coord     = default;
            cluster   = default!;
            meshData1 = default!;
            meshData2 = default!;
            return false;
        }

        #endregion

        #region Completed
        public void EnqueueComplete(
            ChunkCoord coord, 
            ChunkDataCluster cluster,
            VoxelMeshData meshData1,
            VoxelMeshData meshData2
        )
        {
            complete.Enqueue((coord, cluster, meshData1, meshData2));
        }

        public bool TryDequeueComplete(out ChunkCoord coord, out VoxelMeshData opaque, out VoxelMeshData transparent)
        {
            if (complete.TryDequeue(out var pair))
            {
                coord       = pair.coord;
                opaque      = pair.opaque;
                transparent = pair.transparent;
                _clusterPool.Return(pair.cluster);
                return true;
            }

            coord       = default;
            opaque      = default!;
            transparent = default!;
            return false;
        }

        public void ReturnMeshData(VoxelMeshData meshData)
        {
            _meshDataPool.Return(meshData);
        }
        #endregion
    }
}
