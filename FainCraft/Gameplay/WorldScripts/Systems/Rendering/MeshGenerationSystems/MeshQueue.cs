using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;

public class MeshQueue
{
    private readonly PriorityQueue<ChunkCoord, int> _queue = new();
    private readonly HashSet<ChunkCoord> _enqueued = new();
    private readonly Func<RegionCoord> _player;

    public int Count => _enqueued.Count;

    public MeshQueue(Func<RegionCoord> player)
    {
        _player = player;
    }

    public void Enqueue(ChunkCoord coord)
    {
        int priority = CalculatePriority(coord, _player.Invoke());

        if (_enqueued.Add(coord))
            _queue.Enqueue(coord, priority);
    }

    public void Remove(ChunkCoord coord)
    {
        _enqueued.Remove(coord);
    }

    public void Recalculate()
    {
        var playerPos = _player.Invoke();

        // Recalculate priorities and requeue
        _queue.Clear();
        foreach (var coord in _enqueued)
            _queue.Enqueue(coord, CalculatePriority(coord, playerPos));
    }

    public bool TryDequeue(out ChunkCoord coord)
    {
        while (_queue.TryDequeue(out coord, out _))
        {
            if (_enqueued.Remove(coord))
                return true;
        }

        coord = default;
        return false;
    }

    private int CalculatePriority(ChunkCoord coord, RegionCoord player)
    {
        var offset = (RegionCoord)coord - player;
        var chunks = Math.Abs(offset.X) + Math.Abs(offset.Z);

        // Manhatten distance binned to nearest 4 chunks
        return chunks / 4;
    }
}
