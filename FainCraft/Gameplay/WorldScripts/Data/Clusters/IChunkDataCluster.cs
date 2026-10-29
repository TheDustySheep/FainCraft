using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainCraft.Gameplay.WorldScripts.Storage;

namespace FainCraft.Gameplay.WorldScripts.Data;

public interface IChunkDataCluster
{
    public void CopyRefs(ChunkCoord coord, IChunkDataStore chunkDataStore);
    public bool IsEmpty();
    public void CopyData();
    public bool HasVersionsChanged(ChunkCoord coord, IChunkDataStore chunkDataStore);
    public VoxelState GetVoxel(int x, int y, int z);
}