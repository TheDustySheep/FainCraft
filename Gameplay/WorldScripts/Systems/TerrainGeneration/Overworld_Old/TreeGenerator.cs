using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Editing;
using FainEngine_v2.Utils;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld_Old;
internal class TreeGenerator
{
    readonly FastNoiseLite treeNoise = new FastNoiseLite();

    public TreeGenerator()
    {
        treeNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        treeNoise.SetFrequency(1f);
        treeNoise.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2);
    }

    public void GenerateTrees(GenerationData data)
    {
        VoxelData airData = data.Indexer.GetVoxel("Air");
        VoxelData logData = data.Indexer.GetVoxel("Log");
        VoxelData leavesData = data.Indexer.GetVoxel("Leaves");
        VoxelData grassData = data.Indexer.GetVoxel("Grass");

        RegionCoord regionCoord = data.RegionCoord;

        int start_x = regionCoord.Global_Voxel_X;
        int start_z = regionCoord.Global_Voxel_Z;

        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            int global_x = start_x + x;

            for (int z = 0; z < CHUNK_SIZE; z++)
            {
                int global_z = start_z + z;

                float val = treeNoise.GetNoise(global_x, global_z);

                if (val > 0.4)
                {
                    int floorHeight = data.Continentalness.GetHeight(x, z);
                    GlobalVoxelCoord floorCoord = new GlobalVoxelCoord(global_x, floorHeight, global_z);

                    if (!data.GetVoxelData(floorCoord, out var floorData) || floorData != grassData)
                        continue;

                    GlobalVoxelCoord baseCoord = floorCoord + new GlobalVoxelCoord(0, 1, 0);

                    for (int level = 0; level < 5; level++)
                    {
                        GlobalVoxelCoord offsetCoord = baseCoord + new GlobalVoxelCoord(0, level, 0);
                        data.AddVoxelEdit(new ReplaceSingleVoxel(offsetCoord, airData, logData));
                    }

                    int leavesRadius = 2;
                    for (int level = 3; level < 7; level++)
                    {
                        for (int i = -leavesRadius; i <= leavesRadius; i++)
                        {
                            for (int j = -leavesRadius; j <= leavesRadius; j++)
                            {
                                GlobalVoxelCoord offsetCoord = baseCoord + new GlobalVoxelCoord(i, level, j);
                                data.AddVoxelEdit(new ReplaceSingleVoxel(offsetCoord, airData, leavesData));
                            }
                        }

                        if (level > 3)
                            leavesRadius--;
                    }
                }
            }
        }
    }
}