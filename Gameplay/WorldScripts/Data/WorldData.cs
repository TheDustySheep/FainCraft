using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Editing;
using FainCraft.Gameplay.WorldScripts.Voxels;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data;
internal class WorldData : IWorldData
{
    public VoxelIndexer Indexer { get; init; }
    public event Action<ChunkCoord, bool>? OnChunkUpdate;
    private readonly Dictionary<RegionCoord, RegionData> regions = new();

    readonly RegionEditList regionEditList = new RegionEditList();

    public WorldData(VoxelIndexer indexer)
    {
        Indexer = indexer;
    }

    // Voxel
    public void AddRegionEdits(RegionEditList edits)
    {
        var remesh = new List<ChunkCoord>();

        regionEditList.Combine(edits);

        foreach (var pair in regions)
        {
            regionEditList.ApplyEdits(pair.Key, pair.Value, remesh);
        }

        foreach (var chunk in remesh)
        {
            OnChunkUpdate?.Invoke(chunk, false);
        }
    }

    public bool VoxelExists(VoxelCoordGlobal globalCoord)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;
        return GetChunk(chunkCoord) is not null;
    }

    public bool GetVoxelState(VoxelCoordGlobal globalCoord, out VoxelState voxelData)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;

        var chunk = GetChunk(chunkCoord);

        if (chunk is null)
        {
            voxelData = default;
            return false;
        }

        voxelData = chunk[(VoxelCoordLocal)globalCoord];
        return true;
    }

    public bool SetVoxelState(VoxelCoordGlobal globalCoord, VoxelState voxelData)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;

        var chunk = GetChunk(chunkCoord);

        if (chunk is null)
            return false;

        chunk[(VoxelCoordLocal)globalCoord] = voxelData;

        if (OnChunkUpdate is null)
            return true;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(x, y, z), x==0 & y==0 & z==0);
                }
            }
        }

        return true;
    }

    public bool EditVoxelData(VoxelCoordGlobal globalCoord, Func<VoxelState, VoxelState> editFunc)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;

        var chunk = GetChunk(chunkCoord);

        if (chunk is null)
        {
            return false;
        }

        var oldVoxelData = chunk[(VoxelCoordLocal)globalCoord];

        var newVoxelData = editFunc.Invoke(oldVoxelData);

        // If nothing changed then return early
        if (newVoxelData == oldVoxelData)
            return true;

        var localCoord = (VoxelCoordLocal)globalCoord;

        chunk[localCoord] = newVoxelData;

        if (OnChunkUpdate is null)
            return true;

        OnChunkUpdate.Invoke(chunkCoord, true);

        if (localCoord.X == 0)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(-1,  0,  0), false);
        if (localCoord.X == CHUNK_SIZE - 1)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord( 1,  0,  0), false);
        if (localCoord.Y == 0)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord( 0, -1,  0), false);
        if (localCoord.Y == CHUNK_SIZE - 1)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord( 0,  1,  0), false);
        if (localCoord.Z == 0)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord( 0,  0, -1), false);
        if (localCoord.Z == CHUNK_SIZE - 1)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord( 0,  0,  1), false);

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

    public bool UpdateChunk(ChunkCoord chunkCoord, ChunkData data)
    {
        if (!regions.TryGetValue((RegionCoord)chunkCoord, out var regionData))
            return false;

        if (!regionData.SetChunk(chunkCoord.Y, data))
            return false;

        if (OnChunkUpdate is null)
            return true;

        OnChunkUpdate.Invoke(chunkCoord, true);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(-1,  0,  0), false);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord( 1,  0,  0), false);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord( 0, -1,  0), false);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord( 0,  1,  0), false);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord( 0,  0, -1), false);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord( 0,  0,  1), false);

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

    readonly RegionCoord[] REGION_OFFSETS = new[]
    {
        new RegionCoord(-1, 0),
        new RegionCoord( 1, 0),
        new RegionCoord( 0,-1),
        new RegionCoord( 0, 1),
    };

    public bool RegionExists(RegionCoord coord)
    {
        return regions.ContainsKey(coord);
    }

    public bool SetRegion(RegionCoord coord, RegionData data)
    {
        regions[coord] = data;

        foreach (var pair in regions)
        {
            regionEditList.ApplyEdits(pair.Key, pair.Value);
        }

        // Update surrounding chunks
        for (int i = 0; i < 4; i++)
        {
            var offset_coord = REGION_OFFSETS[i] + coord;

            for (int y = 0; y < REGION_Y_TOTAL_COUNT; y++)
            {
                OnChunkUpdate?.Invoke(new ChunkCoord()
                {
                    X = offset_coord.X,
                    Y = y - REGION_Y_NEG_COUNT,
                    Z = offset_coord.Z,
                }, false);
            }
        }

        return true;
    }
}
