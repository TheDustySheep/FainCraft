using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Voxels;

namespace FainCraft.Gameplay.WorldScripts.Chunking;
internal class WorldData : IWorldData
{
    public VoxelIndexer Indexer { get; init; }
    public event Action<ChunkCoord>? OnChunkUpdate;
    private readonly Dictionary<RegionCoord, RegionData> regions = new();

    public WorldData(VoxelIndexer indexer)
    {
        Indexer = indexer;
    }

    // Voxel
    public bool GetVoxelData(GlobalVoxelCoord globalCoord, out VoxelData voxelData)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;

        var chunk = GetChunk(chunkCoord);

        if (chunk is null)
        {
            voxelData = default;
            return false;
        }

        voxelData = chunk[(LocalVoxelCoord)globalCoord];
        return true;
    }

    public bool SetVoxelData(GlobalVoxelCoord globalCoord, VoxelData voxelData)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;

        var chunk = GetChunk(chunkCoord);

        if (chunk is null)
            return false;

        chunk[(LocalVoxelCoord)globalCoord] = voxelData;

        if (OnChunkUpdate is null)
            return true;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(x, y, z));
                }
            }
        }

        return true;
    }

    public bool EditVoxelData(GlobalVoxelCoord globalCoord, Func<VoxelData, VoxelData> editFunc)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;

        var chunk = GetChunk(chunkCoord);

        if (chunk is null)
        {
            return false;
        }

        var oldVoxelData = chunk[(LocalVoxelCoord)globalCoord];

        var newVoxelData = editFunc.Invoke(oldVoxelData);

        // If nothing changed then return early
        if (newVoxelData == oldVoxelData)
            return true;

        chunk[(LocalVoxelCoord)globalCoord] = newVoxelData;

        if (OnChunkUpdate is null)
            return true;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(x, y, z));
                }
            }
        }

        return true;
    }

    // Chunk
    public ChunkData? GetChunk(ChunkCoord coord)
    {
        if (regions.TryGetValue((RegionCoord)coord, out var regionData))
        {
            return regionData.GetChunk(coord.Y);
        }

        return null;
    }

    public bool UpdateChunk(ChunkCoord coord, ChunkData data)
    {
        if (!regions.TryGetValue((RegionCoord)coord, out var regionData))
            return false;

        if (!regionData.SetChunk(coord.Y, data))
            return false;

        if (OnChunkUpdate is null)
            return true;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    OnChunkUpdate.Invoke(coord + new ChunkCoord(x, y, z));
                }
            }
        }

        return true;
    }

    // Cluster
    public ChunkData?[] GetCluster(ChunkCoord coord)
    {
        var result = new ChunkData?[27];
        int i = 0;
        for (int z = -1; z < 2; z++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++, i++)
                {
                    var offset = new ChunkCoord(x, y, z);
                    var _coord = offset + coord;
                    result[i] = GetChunk(_coord);
                }
            }
        }

        return result;
    }

    // Region
    public RegionData? GetRegion(RegionCoord coord)
    {
        regions.TryGetValue(coord, out var regionData);
        return regionData;
    }

    public bool SetRegion(RegionCoord coord, RegionData data)
    {
        regions[coord] = data;

        if (OnChunkUpdate is null)
            return true;

        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                var offset_coord = new RegionCoord(x, z) + coord;

                for (int y = 0; y < WorldConstants.REGION_Y_TOTAL_COUNT; y++)
                {
                    OnChunkUpdate.Invoke(new ChunkCoord()
                    {
                        X = offset_coord.X,
                        Y = y - WorldConstants.REGION_Y_NEG_COUNT,
                        Z = offset_coord.Z,
                    });
                }
            }
        }
        return true;
    }
}
