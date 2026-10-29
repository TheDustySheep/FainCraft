using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Storage;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;
using FainEngine_v2.Core;
using FainEngine_v2.Utils;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;

internal class MeshGenerationSystem_v2 : IMeshGenerationSystem
{
    public event Action<ChunkCoord, VoxelMeshData, VoxelMeshData>? OnComplete;

    private readonly MeshQueue _meshQueue;
    private readonly Queue<GenerationData> _pool = new();
    private readonly ConcurrentQueue<GenerationData> _input = new();
    private readonly ConcurrentQueue<GenerationData> _output = new();

    public MeshGenerationSystem_v2(Func<RegionCoord> playerPos, IChunkDataStore chunkStore, Func<IMeshGenerator> generatorFactory)
    {
        _meshQueue = new(playerPos);
    }

    public void Tick()
    {

    }

    public void Update(ChunkCoord coord)
    {
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

    private struct Request
    {
        public ChunkCoord coord;
        //public 
    }
}
