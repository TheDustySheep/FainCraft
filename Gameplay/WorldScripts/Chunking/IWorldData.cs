using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Voxels;

namespace FainCraft.Gameplay.WorldScripts.Chunking;
internal interface IWorldData
{
    public event Action<ChunkCoord>? OnChunkUpdate;

    public VoxelIndexer Indexer { get; }

    // Voxel
    public bool GetVoxelData(GlobalVoxelCoord coord, out VoxelData voxelData);
    public bool SetVoxelData(GlobalVoxelCoord coord, VoxelData voxelData);
    public bool EditVoxelData(GlobalVoxelCoord coord, Func<VoxelData, VoxelData> editFunc);

    // Chunk
    public ChunkData? GetChunk(ChunkCoord coord);
    public bool UpdateChunk(ChunkCoord coord, ChunkData data);

    // Cluster
    public ChunkData?[] GetCluster(ChunkCoord coord);

    // Region
    public RegionData? GetRegion(RegionCoord coord);
    public bool SetRegion(RegionCoord coord, RegionData data);
}