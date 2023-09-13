using FainCraft.Gameplay.WorldScripts.Core;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld;
internal class SurfaceDecorator
{
    readonly HeightMapGenerator heightMapGenerator = new();

    public SurfaceDecorator()
    {
    }

    public void DecorateSurface(GenerationData data)
    {
        heightMapGenerator.GenerateHeightMap(data);
        for (int y = 0; y < REGION_Y_TOTAL_COUNT; y++)
        {
            GenerateChunk(
                data,
                new ChunkCoord(
                    data.RegionCoord.X,
                    y - REGION_Y_NEG_COUNT,
                    data.RegionCoord.Z)
                );
        }
    }

    private void GenerateChunk(GenerationData data, ChunkCoord chunkCoord)
    {
        var air = data.Indexer.GetVoxel("Air");
        var stone = data.Indexer.GetVoxel("Stone");
        var grass = data.Indexer.GetVoxel("Grass");
        var dirt = data.Indexer.GetVoxel("Dirt");
        var sand = data.Indexer.GetVoxel("Sand");
        var water = data.Indexer.GetVoxel("Water");

        GlobalVoxelCoord chunkGlobalCoord = (GlobalVoxelCoord)chunkCoord;

        for (int z = 0; z < CHUNK_SIZE; z++)
        {
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    GlobalVoxelCoord globalCoord = chunkGlobalCoord + new GlobalVoxelCoord(x, y, z);

                    if (!data.GetVoxelData(globalCoord, out var currentVoxel) || currentVoxel != stone)
                        continue;

                    if (data.GetVoxelData(globalCoord + new GlobalVoxelCoord(0, 1, 0), out var aboveVoxel) && aboveVoxel != air)
                        continue;

                    data.SetVoxelData(globalCoord, grass);
                    data.SetVoxelData(globalCoord + new GlobalVoxelCoord(0, -1, 0), dirt);
                    data.SetVoxelData(globalCoord + new GlobalVoxelCoord(0, -2, 0), dirt);
                    data.SetVoxelData(globalCoord + new GlobalVoxelCoord(0, -3, 0), dirt);
                }
            }
        }
    }
}
