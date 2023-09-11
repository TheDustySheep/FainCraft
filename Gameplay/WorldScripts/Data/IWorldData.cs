using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Editing;
using FainCraft.Gameplay.WorldScripts.Voxels;

namespace FainCraft.Gameplay.WorldScripts.Data;
internal interface IWorldData : IVoxelEditable
{
    public event Action<ChunkCoord, bool>? OnChunkUpdate;

    public VoxelIndexer Indexer { get; }

    public void AddVoxelEdits(IEnumerable<IVoxelEdit> edits);

    // Chunk
    public ChunkData? GetChunk(ChunkCoord coord);
    public bool UpdateChunk(ChunkCoord coord, ChunkData data, bool immediate = false);

    // Cluster
    public ChunkData?[] GetCluster(ChunkCoord coord);

    // Region
    public RegionData? GetRegion(RegionCoord coord);
    public bool SetRegion(RegionCoord coord, RegionData data, bool immediate = false);
}