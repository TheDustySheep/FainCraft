using FainCraft.Gameplay.WorldScripts.Coords;
using FainEngine_v2.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;


/// <summary>
/// Encapsulates queueing, semaphore, and worker-thread logic.
/// </summary>
internal class MeshGenerationQueue : IDisposable
{
    private readonly PriorityHashQueue<ChunkCoord, int> _requestQueue = new PriorityHashQueue<ChunkCoord, int>();
    private readonly ConcurrentQueue<GenerationData> _toGenerate = new ConcurrentQueue<GenerationData>();
    private readonly ConcurrentQueue<(ChunkCoord coord, GenerationData data)> _completed = new ConcurrentQueue<(ChunkCoord, GenerationData)>();
    private readonly SemaphoreSlim _semaphore;
    private readonly Thread _worker;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public MeshGenerationQueue(int maxActive)
    {
        _semaphore = new SemaphoreSlim(maxActive, maxActive);
        _worker = new Thread(WorkerLoop) { IsBackground = true };
    }

    public int Count => _requestQueue.Count;

    public void Start() => _worker.Start();

    #region Requests
    public void EnqueueRequest(ChunkCoord coord, int priority)
        => _requestQueue.Enqueue(coord, priority);

    public bool TryDequeueRequest(out ChunkCoord coord)
    {
        coord = default;

        if (_requestQueue.Count == 0)
            return false;

        if (!_semaphore.Wait(0))
            return false;

        if (!_requestQueue.TryDequeue(out coord, out _))
        {
            _semaphore.Release();
            return false;
        }

        return true;
    }

    public void ReleaseSlot() 
        => _semaphore.Release();
    #endregion

    public void EnqueueToGenerate(GenerationData data)
        => _toGenerate.Enqueue(data);

    public bool TryDequeueToGenerate(out GenerationData data)
        => _toGenerate.TryDequeue(out data);

    public bool TryDequeueCompleted(out (ChunkCoord coord, GenerationData data) result)
        => _completed.TryDequeue(out result);

    private void WorkerLoop()
    {
        while (!_cts.IsCancellationRequested)
        {
            if (_toGenerate.TryDequeue(out var genData))
            {
                try
                {
                    genData.Generator.GenerateMesh(genData.ClusterData, genData.Opaque, genData.Transparent);
                    _completed.Enqueue((genData.Coord, genData));
                }
                catch
                {
                    _semaphore.Release();
                }
            }
            else Thread.Sleep(1);
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _worker.Join();
        _semaphore.Dispose();
        _cts.Dispose();
    }
}