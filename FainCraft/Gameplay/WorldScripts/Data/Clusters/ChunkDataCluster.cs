using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainCraft.Gameplay.WorldScripts.Storage;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data.Clusters;
public class ChunkDataCluster : IChunkDataCluster
{
    readonly uint?[] _chunkVersions = new uint?[27];
    readonly ChunkDataRef?[] _chunkRefs = new ChunkDataRef?[27];
    readonly VoxelState[] _voxelData = new VoxelState[27*CHUNK_VOLUME];

    public void CopyRefs(ChunkCoord coord, IChunkDataStore chunkDataStore)
    {
        int i = 0;
        for (int y = -1; y < 2; y++)
        {
            for (int z = -1; z < 2; z++)
            {
                for (int x = -1; x < 2; x++, i++)
                {
                    var offset = new ChunkCoord(x, y, z) + coord;
                    chunkDataStore.GetChunkData(offset, out _chunkRefs[i]);
                }
            }
        }
    }

    public bool IsEmpty()
    {
        var chunkRef = _chunkRefs[13];
        return chunkRef == null || chunkRef.IsEmpty();
    }

    public void CopyData()
    {
        Span<VoxelState> voxelData = _voxelData.AsSpan();
        voxelData.Clear();

        for (int i = 0; i < 27; i++)
        {
            Span<VoxelState> dst = voxelData.Slice(i * CHUNK_VOLUME, CHUNK_VOLUME);
            var chunkRef = _chunkRefs[i];

            if (chunkRef == null)
            {
                _chunkVersions[i] = null;
            }
            else
            {
                _chunkVersions[i] = chunkRef.CopyTo(dst);
            }
        }
    }

    public bool HasVersionsChanged(ChunkCoord coord, IChunkDataStore chunkDataStore)
    {
        int i = 0;
        for (int y = -1; y < 2; y++)
        {
            for (int z = -1; z < 2; z++)
            {
                for (int x = -1; x < 2; x++, i++)
                {
                    var offset = new ChunkCoord(x, y, z) + coord;
                    chunkDataStore.GetChunkData(offset, out var _nChunkRef);

                    // World Chunk has Changed
                    if (_chunkRefs[i] != _nChunkRef)
                        return true;

                    if (_nChunkRef?.VersionID != _chunkVersions[i])
                        return true;
                }
            }
        }
        return false;
    }

    public VoxelState GetVoxel(int x, int y, int z)
    {
        int chunkIndex = 13 +
            (x >> CHUNK_SIZE_POWER) +
            (z >> CHUNK_SIZE_POWER) * 3 +
            (y >> CHUNK_SIZE_POWER) * 9;

        int localIndex = ChunkIndex
        (
            x + CHUNK_SIZE & CHUNK_SIZE_MASK,
            y + CHUNK_SIZE & CHUNK_SIZE_MASK,
            z + CHUNK_SIZE & CHUNK_SIZE_MASK
        );

        return _voxelData[chunkIndex * CHUNK_VOLUME + localIndex];
    }
}