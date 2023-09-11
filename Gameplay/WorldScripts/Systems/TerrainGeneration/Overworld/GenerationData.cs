using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Editing;
using FainCraft.Gameplay.WorldScripts.Voxels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld;
internal class GenerationData : IVoxelEditable
{
    public List<IVoxelEdit> outstandingEdits = new();

    public const int BORDER = 8;
    public const int MAP_SIZE = BORDER + CHUNK_SIZE + BORDER;
    public Image<HalfSingle> Image = new(MAP_SIZE, MAP_SIZE);
    public ushort[] HeightMap = new ushort[MAP_SIZE * MAP_SIZE];

    public VoxelIndexer Indexer;

    public RegionCoord RegionCoord;
    public RegionData regionData = new();

    public GenerationData(VoxelIndexer indexer)
    {
        Indexer = indexer;
    }

    public ChunkData[] Chunks => regionData.Chunks;

    public void Initalize(RegionCoord regionCoord)
    {
        RegionCoord = regionCoord;
        regionData = new();
        outstandingEdits.Clear();
    }

    public ushort GetHeight(int x, int z)
    {
        x += BORDER;
        z += BORDER;

        return HeightMap[z * MAP_SIZE + x];
    }

    public bool VoxelExists(GlobalVoxelCoord coord)
    {
        ChunkCoord chunkCoord = (ChunkCoord)coord;

        return
            new RegionCoord(coord) == RegionCoord &&
            regionData.GetChunk(chunkCoord.Y) is not null;
    }

    public void AddVoxelEdit(IVoxelEdit edit)
    {
        ChunkCoord chunkCoord = (ChunkCoord)edit.GlobalCoord;

        if (new RegionCoord(chunkCoord) != RegionCoord)
        {
            outstandingEdits.Add(edit);
            return;
        }

        var chunk = regionData.GetChunk(chunkCoord.Y);

        // Above or below world then forget about it lol
        if (chunk is null)
            return;

        edit.Execute(this);
    }

    public bool GetVoxelData(GlobalVoxelCoord globalCoord, out VoxelData voxel)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;

        if (new RegionCoord(chunkCoord) != RegionCoord)
        {
            voxel = default;
            return false;
        }

        var chunk = regionData.GetChunk(chunkCoord.Y);

        if (chunk is null)
        {
            voxel = default;
            return false;
        }

        voxel = chunk[(LocalVoxelCoord)globalCoord];
        return true;
    }

    public bool SetVoxelData(GlobalVoxelCoord globalCoord, VoxelData newVoxelData, bool immediate = false)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;

        if (new RegionCoord(chunkCoord) != RegionCoord)
            return false;

        var chunk = regionData.GetChunk(chunkCoord.Y);

        if (chunk is null)
            return false;

        chunk[(LocalVoxelCoord)globalCoord] = newVoxelData;
        return true;
    }

    public bool EditVoxelData(GlobalVoxelCoord globalCoord, Func<VoxelData, VoxelData> editFunc, bool immediate = false)
    {
        ChunkCoord chunkCoord = (ChunkCoord)globalCoord;

        if (new RegionCoord(chunkCoord) != RegionCoord)
            return false;

        var chunk = regionData.GetChunk(chunkCoord.Y);

        if (chunk is null)
            return false;

        int localIndex = ((LocalVoxelCoord)globalCoord).VoxelIndex;
        chunk[localIndex] = editFunc.Invoke(chunk[localIndex]);
        return true;
    }
}
