using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Voxels;
using System.Diagnostics;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld_Old;
internal class OverworldGenerator : ITerrainGenerator
{
    const int stoneDepth = 4;
    const int waterHeight = 36;

    readonly VoxelIndexer indexer;
    readonly TreeGenerator treeGenerator = new();
    readonly GenerationData data;
    readonly TerrainShaper terrainShaper = new();
    readonly SurfaceDecorator surfaceDecorator = new();

    public OverworldGenerator(VoxelIndexer indexer)
    {
        this.indexer = indexer;
        data = new GenerationData(indexer);
    }

    public RegionGenerationResult Generate(RegionCoord regionCoord)
    {
        Stopwatch sw = Stopwatch.StartNew();
        data.Initalize(regionCoord);

        terrainShaper.ShapeTerrain(data);
        surfaceDecorator.DecorateSurface(data);
        treeGenerator.GenerateTrees(data);

        sw.Stop();
        SystemDiagnostics.SubmitTerrainGeneration(new TerrainDebugData() { TotalTime = sw.Elapsed });

        return new RegionGenerationResult(data);
    }

    private ChunkData GenerateChunk(GenerationData data, ChunkCoord chunkCoord)
    {
        VoxelData Air = new() { Index = indexer.GetIndex("Air") };
        VoxelData Grass = new() { Index = indexer.GetIndex("Grass") };
        VoxelData Dirt = new() { Index = indexer.GetIndex("Dirt") };
        VoxelData Sand = new() { Index = indexer.GetIndex("Sand") };
        VoxelData Stone = new() { Index = indexer.GetIndex("Stone") };
        VoxelData Water = new() { Index = indexer.GetIndex("Water") };

        GlobalVoxelCoord chunkVoxelCoord = (GlobalVoxelCoord)chunkCoord;

        var chunkData = new ChunkData();

        for (int c_z = 0; c_z < CHUNK_SIZE; c_z++)
        {
            int global_z = c_z + chunkVoxelCoord.Z;

            for (int c_x = 0; c_x < CHUNK_SIZE; c_x++)
            {
                int global_x = c_x + chunkVoxelCoord.X;

                int groundHeight = data.Continentalness.GetHeight(c_x, c_z);

                for (int c_y = 0; c_y < CHUNK_SIZE; c_y++)
                {
                    int global_y = c_y + chunkVoxelCoord.Y;

                    VoxelData voxData;

                    if (global_y > groundHeight)
                    {
                        if (global_y <= waterHeight)
                            voxData = Water;
                        else
                            voxData = Air;
                    }
                    else if (global_y == groundHeight)
                    {
                        if (global_y <= waterHeight + 2)
                            voxData = Sand;
                        else
                            voxData = Grass;
                    }
                    else if (global_y > groundHeight - stoneDepth)
                    {
                        if (global_y <= waterHeight)
                            voxData = Sand;
                        else
                            voxData = Dirt;
                    }
                    else
                        voxData = Stone;

                    chunkData[c_x, c_y, c_z] = voxData;
                }
            }
        }
        return chunkData;
    }
}
