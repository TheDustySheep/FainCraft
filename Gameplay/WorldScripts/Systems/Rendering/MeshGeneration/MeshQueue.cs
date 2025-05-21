using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Data.Clusters;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;
using FainEngine_v2.Collections;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration
{
    public class MeshQueue
    {
        public int BufferCount => _RequestInBuffer.Count;

        readonly ObjectPoolFactory<IChunkDataCluster> _clusterPool = new(() => new ChunkDataClusterFull()) { MaxItems = 128 };
        readonly ObjectPool<VoxelMeshData>    _meshDataPool = new() { MaxItems = 128 };

        readonly OrderedSet<ChunkCoord> _RequestInBuffer = new();
        readonly ConcurrentQueue<(ChunkCoord coord, IChunkDataCluster cluster)> _GenInBuffer = new();
        readonly ConcurrentQueue<(ChunkCoord coord, IChunkDataCluster cluster, VoxelMeshData opaque, VoxelMeshData transparent)> complete = new();

        #region Requests
        public void EnqueueRequest(ChunkCoord chunkCoord)
        {
            _RequestInBuffer.AddLast(chunkCoord);
        }

        public bool TryDequeueRequest(out ChunkCoord coord, out IChunkDataCluster cluster)
        {
            if (_GenInBuffer.Count >= SharedVariables.RenderSettings.Value.MaxConcurrentMeshes ||
                !_RequestInBuffer.TryDequeue(out coord))
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
        
        public void EnqueueGeneration(ChunkCoord coord, IChunkDataCluster cluster)
        {
            _GenInBuffer.Enqueue((coord, cluster));
        }

        public bool TryDequeueGeneration(
            out ChunkCoord coord, 
            out IChunkDataCluster cluster, 
            out VoxelMeshData meshData1,
            out VoxelMeshData meshData2
        )
        {
            if (_GenInBuffer.TryDequeue(out var pair))
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
            IChunkDataCluster cluster,
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
