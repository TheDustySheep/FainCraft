using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Editing;
using FainCraft.Gameplay.WorldScripts.Voxels;

namespace FainCraft.Gameplay.WorldScripts.Data;
public interface IWorldData
{
    public event Action<ChunkCoord, bool>? OnChunkUpdate;

    public IVoxelIndexer Indexer { get; }

    public void AddRegionEdits(RegionEditList edits);

    // Chunk
    public ChunkData? GetChunk(ChunkCoord coord);
    public bool UpdateChunk(ChunkCoord coord, ChunkData data);

    // Cluster
    public ChunkData?[] GetCluster(ChunkCoord coord);

    // Region
    public RegionData? GetRegion(RegionCoord coord);
    public bool RegionExists(RegionCoord coord);
    public bool SetRegion(RegionCoord coord, RegionData data);
    public bool UnloadRegion(RegionCoord coord, out RegionData data);

    // Voxels
    bool VoxelExists(VoxelCoordGlobal globalCoord);
    bool GetVoxelState(VoxelCoordGlobal globalCoord, out VoxelState voxelData);
    bool SetVoxelState(VoxelCoordGlobal globalCoord, VoxelState voxelData);
    bool EditVoxelState(VoxelCoordGlobal globalCoord, Func<VoxelState, VoxelState> editFunc);
}