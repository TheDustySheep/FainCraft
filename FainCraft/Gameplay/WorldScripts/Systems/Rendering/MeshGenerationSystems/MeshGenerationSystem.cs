using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Storage;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;
using FainEngine_v2.Core;
using FainEngine_v2.Utils;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;

/// <summary>
/// Manages _mesh generation for chunks in a voxel world.
/// Uses MeshGenerationQueue to handle requests, concurrency, and threading.
/// </summary>
internal class MeshGenerationSystem : IMeshGenerationSystem, IDisposable
{
    private const int POOL_SIZE = 32;

    private readonly IGameTime _gameTime;

    private readonly Queue<GenerationData> _pool = new();

    private readonly ConcurrentQueue<GenerationData> _input = new();
    private readonly ConcurrentQueue<GenerationData> _output = new();

    private Func<int> _maxApply;
    private readonly MeshQueue _meshQueue;

    private readonly IChunkDataStore _chunkStore;
    private readonly IChunkClusterDataStore _clusterDataStore;
    private readonly VoxelMeshData _emptyMeshData = new();

    private readonly List<MeshingThread> _threads = new();

    public event Action<ChunkCoord, VoxelMeshData, VoxelMeshData>? OnComplete;

    public MeshGenerationSystem(Func<RegionCoord> playerPos, IChunkDataStore chunkStore, IChunkClusterDataStore clusterStore, Func<IMeshGenerator> generatorFactory)
    {
        _chunkStore = chunkStore;
        _clusterDataStore = clusterStore;
     
        _gameTime = DependencyInjector.Resolve<IGameTime>();
        _meshQueue = new(playerPos);

        // Dependencies
        _maxApply = () => (int)SharedVariables.RenderSettings.Value.MeshesAppliedPerFrame;

        // Generation Data Pool
        for (int i = 0; i < POOL_SIZE; i++)
            _pool.Enqueue(new GenerationData());

        // Meshing Threads
        for (int i = 0; i < 4; i++)
        {
            _threads.Add(new MeshingThread(
                generatorFactory.Invoke(),
                _input,
                _output,
                $"Mesh Generation Worker {i+1}"
            ));
        }
    }
    ~MeshGenerationSystem() => Dispose();

    public void Update(ChunkCoord coord)
    {
        // Early cull of empty chunks
        if (!_chunkStore.GetChunkData(coord, out var data) || data.IsEmpty())
        {
            OnComplete?.Invoke(coord, _emptyMeshData, _emptyMeshData);
            _meshQueue.Remove(coord);
            return;
        }

        _meshQueue.Enqueue(coord);
    }

    public void Update(RegionCoord coord)
    {
        foreach (var y in WorldConstants.Iterate_Y_Chunks())
            _meshQueue.Enqueue(new ChunkCoord(coord, y));
    }

    public void Cancel(RegionCoord coord)
    {
        foreach (var y in WorldConstants.Iterate_Y_Chunks())
            _meshQueue.Remove(new ChunkCoord(coord, y));
    }

    public void Tick()
    {
        // Recalculate every 100 ticks
        if (_gameTime.TotalTicks % 100 == 0)
            _meshQueue.Recalculate();


        // Push data into the input queue
        for (int i = 0; i < POOL_SIZE; i++)
        {
            // Pool Empty
            if (!_pool.TryDequeue(out var gen))
                break;

            // Mesh Queue Empty
            if (!_meshQueue.TryDequeue(out var coord))
            {
                _pool.Enqueue(gen);
                break;
            }

            // Failed to find the chunk data
            if (!_clusterDataStore.GetChunkClusterData(coord, gen.DataArray))
            {
                _pool.Enqueue(gen);
                continue;
            }

            gen.Coord = coord;
            gen.ClusterData.SetData(gen.DataArray);

            _input.Enqueue(gen);
        }

        // Process data in the output queue
        int maxApply = _maxApply.Invoke();
        for (int i = 0; i < maxApply; i++)
        {
            if (!_output.TryDequeue(out var gen))
                break;

            OnComplete?.Invoke(gen.Coord, gen.Opaque, gen.Transparent);

            _pool.Enqueue(gen);
        }
    }

    public void Dispose()
    {
        foreach (var thread in _threads)
            thread.Dispose();

        GC.SuppressFinalize(this);
    }

    private class MeshingThread : IDisposable
    {
        private readonly IMeshGenerator _generator;
        private readonly Thread _thread;
        private readonly ConcurrentQueue<GenerationData> _input;
        private readonly ConcurrentQueue<GenerationData> _output;
        private bool _disposed;

        public MeshingThread(IMeshGenerator generator, ConcurrentQueue<GenerationData> input, ConcurrentQueue<GenerationData> output, string threadName)
        {
            _generator = generator;
            _input = input;
            _output = output;

            _thread = new Thread(Tick);
            _thread.Start();
            _thread.Name = threadName;
        }

        ~MeshingThread() => Dispose();
        public void Dispose() => _disposed = true;

        private void Tick()
        {
            while (!_disposed)
            {
                if (_input.TryDequeue(out var data))
                {
                    _generator.GenerateMesh(
                        data.ClusterData,
                        data.Opaque,
                        data.Transparent
                    );

                    _output.Enqueue(data);
                }
                else
                    Thread.Sleep(100);
            }
        }
    }
}