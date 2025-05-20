using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts.Coords;
using FainEngine_v2.Utils.Variables;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;

internal class ThreadedTerrainGenerationSystem : ITerrainGenerationSystem, IDisposable
{
    private readonly ReferenceVariable<PlayerPosition> _playerPosition = SharedVariables.PlayerPosition;
    private readonly ITerrainGenerator _generator;

    // Internal queue item
    private class Request
    {
        public RegionCoord Coord { get; }
        public TaskCompletionSource<RegionGenerationResult> Tcs { get; }
        public CancellationToken Token { get; }

        public Request(RegionCoord coord,
                       TaskCompletionSource<RegionGenerationResult> tcs,
                       CancellationToken token)
        {
            Coord = coord;
            Tcs = tcs;
            Token = token;
        }
    }

    // Priority queue keyed by distance (uint)
    private readonly PriorityQueue<Request, uint> _queue = new PriorityQueue<Request, uint>();
    private readonly object _lock = new object();
    private readonly AutoResetEvent _signal = new AutoResetEvent(false);
    private readonly Thread _workerThread;
    private bool _disposed;

    public ThreadedTerrainGenerationSystem(IServiceProvider provider)
    {
        _generator = provider.Get<ITerrainGenerator>();
        _workerThread = new Thread(WorkLoop)
        {
            IsBackground = true,
            Name = "TerrainGenWorker"
        };
        _workerThread.Start();
    }

    public Task<RegionGenerationResult> GenerateAsync(RegionCoord coord, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<RegionGenerationResult>(token);

        var tcs = new TaskCompletionSource<RegionGenerationResult>(
                      TaskCreationOptions.RunContinuationsAsynchronously);
        var request = new Request(coord, tcs, token);
        var priority = Distance(coord);

        lock (_lock)
        {
            _queue.Enqueue(request, priority);
            _signal.Set();
        }

        return tcs.Task;
    }

    private void WorkLoop()
    {
        while (true)
        {
            Request next = null;

            lock (_lock)
            {
                if (_disposed)
                    return;

                if (_queue.Count == 0)
                {
                    // wait until there's something new (or Dispose signals)
                    Monitor.Exit(_lock);
                    _signal.WaitOne();
                    Monitor.Enter(_lock);
                }

                if (_disposed)
                    return;

                if (_queue.Count > 0)
                    next = _queue.Dequeue();
            }

            if (next != null)
            {
                // honor individual cancellation
                if (next.Token.IsCancellationRequested)
                {
                    next.Tcs.TrySetCanceled(next.Token);
                    continue;
                }

                try
                {
                    var result = _generator.Generate(next.Coord);
                    next.Tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    next.Tcs.TrySetException(ex);
                }
            }
        }
    }

    private uint Distance(RegionCoord coord)
        => _playerPosition.Value.RegionCoord.ManhattenDistance(coord);

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed) return;
            _disposed = true;
            _signal.Set();
        }
        _workerThread.Join();
        _signal.Dispose();
    }
}