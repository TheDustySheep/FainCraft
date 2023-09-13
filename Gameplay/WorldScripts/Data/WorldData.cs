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

    readonly Dictionary<RegionCoord, List<IVoxelEdit>> voxelEdits = new();

    public WorldData(VoxelIndexer indexer)
    {
        Indexer = indexer;
    }

    // Voxel
    public void AddVoxelEdits(IEnumerable<IVoxelEdit> edits)
    {
        foreach (var edit in edits)
        {
            RegionCoord regionCoord = (RegionCoord)edit.GlobalCoord;

            if (regions.ContainsKey(regionCoord))
            {
                edit.Execute(this);
            }
            else
            {
                if (!voxelEdits.TryGetValue(regionCoord, out var editList))
                {
                    editList = new List<IVoxelEdit>();
                    voxelEdits[regionCoord] = editList;
                }
                editList.Add(edit);
            }
        }
    }

    public bool VoxelExists(GlobalVoxelCoord globalCoord)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;
        return GetChunk(chunkCoord) is not null;
    }

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

    public bool SetVoxelData(GlobalVoxelCoord globalCoord, VoxelData voxelData, bool immediate = false)
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
                    OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(x, y, z), immediate);
                }
            }
        }

        return true;
    }

    public bool EditVoxelData(GlobalVoxelCoord globalCoord, Func<VoxelData, VoxelData> editFunc, bool immediate = false)
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

        var localCoord = (LocalVoxelCoord)globalCoord;

        chunk[localCoord] = newVoxelData;

        if (OnChunkUpdate is null)
            return true;

        OnChunkUpdate.Invoke(chunkCoord, immediate);

        if (localCoord.X == 0)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(-1, 0, 0), immediate);
        if (localCoord.X == CHUNK_SIZE - 1)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(1, 0, 0), immediate);
        if (localCoord.Y == 0)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(0, -1, 0), immediate);
        if (localCoord.Y == CHUNK_SIZE - 1)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(0, 1, 0), immediate);
        if (localCoord.Z == 0)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(0, 0, -1), immediate);
        if (localCoord.Z == CHUNK_SIZE - 1)
            OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(0, 0, 1), immediate);

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

    public bool UpdateChunk(ChunkCoord chunkCoord, ChunkData data, bool immediate = false)
    {
        if (!regions.TryGetValue((RegionCoord)chunkCoord, out var regionData))
            return false;

        if (!regionData.SetChunk(chunkCoord.Y, data))
            return false;

        if (OnChunkUpdate is null)
            return true;

        OnChunkUpdate.Invoke(chunkCoord, immediate);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(-1, 0, 0), immediate);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(1, 0, 0), immediate);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(0, -1, 0), immediate);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(0, 1, 0), immediate);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(0, 0, -1), immediate);
        OnChunkUpdate.Invoke(chunkCoord + new ChunkCoord(0, 0, 1), immediate);

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

    public bool SetRegion(RegionCoord coord, RegionData data, bool immediate = false)
    {
        regions[coord] = data;

        if (voxelEdits.TryGetValue(coord, out var editList))
        {
            foreach (var edit in editList)
            {
                edit.Execute(this);
            }
            voxelEdits.Remove(coord);
        }

        if (OnChunkUpdate is null)
            return true;

        for (int i = 0; i < 4; i++)
        {
            var offset_coord = REGION_OFFSETS[i] + coord;

            for (int y = 0; y < REGION_Y_TOTAL_COUNT; y++)
            {
                OnChunkUpdate.Invoke(new ChunkCoord()
                {
                    X = offset_coord.X,
                    Y = y - REGION_Y_NEG_COUNT,
                    Z = offset_coord.Z,
                }, immediate);
            }
        }

        return true;
    }
}
