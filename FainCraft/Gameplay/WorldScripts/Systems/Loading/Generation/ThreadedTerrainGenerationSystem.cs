using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainEngine_v2.Utils.Variables;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;

internal class ThreadedTerrainGenerationSystem : ITerrainGenerationSystem, IDisposable
{
    private readonly PriorityQueue<(RegionCoord Coord, TaskCompletionSource<RegionData> Tcs), uint> _pending = new();
    private readonly ReferenceVariable<PlayerPosition> _playerPosition = SharedVariables.PlayerPosition;
    private readonly ITerrainGenerator _generator;
    private readonly Thread _workerThread;
    private readonly object _lock = new();
    private bool _disposed;

    public ThreadedTerrainGenerationSystem(ITerrainGenerator generator)
    {
        _generator = generator;
        _workerThread = new Thread(WorkLoop)
        {
            IsBackground = true,
            Name = "TerrainGenWorker"
        };
        _workerThread.Start();
    }

    public Task<RegionData> GenerateAsync(RegionCoord coord)
    {
        var tcs = new TaskCompletionSource<RegionData>(TaskCreationOptions.RunContinuationsAsynchronously);
        var priority = Distance(coord);

        lock (_lock)
        {
            _pending.Enqueue((coord, tcs), priority);
        }

        return tcs.Task;
    }

    private void WorkLoop()
    {
        while (!_disposed)
        {
            bool found;
            RegionCoord coord;
            TaskCompletionSource<RegionData> tcs;

            lock (_lock)
            {
                found = _pending.TryDequeue(out var data, out var priority);
                coord = data.Coord;
                tcs = data.Tcs;
            }

            if (found)
            {
                var regionData = _generator.Generate(coord);
                tcs.TrySetResult(regionData);
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }

    private uint Distance(RegionCoord coord) => _playerPosition.Value.RegionCoord.ManhattenDistance(coord);

    public void Dispose()
    {
        _disposed = true;
    }
}