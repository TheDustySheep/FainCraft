using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Editing;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.HeightMaps;
using FainCraft.Gameplay.WorldScripts.Voxels;
using FainEngine_v2.Utils;
using System;
using System.Diagnostics;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld
{
    internal class OverworldGenerator : ITerrainGenerator
    {
        readonly VoxelIndexer _indexer;

        readonly HeightMapGenerator heightMapGenerator;

        readonly VoxelData Air;
        readonly VoxelData Grass;
        readonly VoxelData Dirt;
        readonly VoxelData Sand;
        readonly VoxelData Stone;
        readonly VoxelData Water;

        public OverworldGenerator(VoxelIndexer indexer, int seed=1337)
        {
            _indexer = indexer;

            heightMapGenerator = new HeightMapGenerator(seed);

            Air   = new() { Index = indexer.GetIndex("Air")   };
            Grass = new() { Index = indexer.GetIndex("Grass") };
            Dirt  = new() { Index = indexer.GetIndex("Dirt")  };
            Sand  = new() { Index = indexer.GetIndex("Sand")  };
            Stone = new() { Index = indexer.GetIndex("Stone") };
            Water = new() { Index = indexer.GetIndex("Water") };
        }

        public RegionGenerationResult Generate(RegionCoord regionCoord)
        {
            Stopwatch sw = Stopwatch.StartNew();
            GlobalVoxelCoord regionGlobal = (GlobalVoxelCoord)regionCoord;

            var regionData = new RegionData();

            heightMapGenerator.Generate(regionCoord);

            for (int y = 0; y < REGION_Y_TOTAL_COUNT; y++)
            {
                var chunkCoord = new ChunkCoord(regionCoord, y - REGION_Y_NEG_COUNT);

                GenerateChunk(regionData.Chunks[y], chunkCoord);
            }

            sw.Stop();
            SystemDiagnostics.SubmitTerrainGeneration(new TerrainDebugData() { TotalTime = sw.Elapsed });

            return new RegionGenerationResult(regionData, new List<IVoxelEdit>());
        }

        private void GenerateChunk(ChunkData data, ChunkCoord chunkCoord)
        {
            for (int l_x = 0; l_x < CHUNK_SIZE; l_x++)
            {
                for (int l_z = 0; l_z < CHUNK_SIZE; l_z++)
                {
                    int height = heightMapGenerator.GetHeight(l_x, l_z);

                    for (int l_y = 0; l_y < CHUNK_SIZE; l_y++)
                    {
                        LocalVoxelCoord  localCoord  = new(l_x, l_y, l_z);
                        GlobalVoxelCoord globalCoord = new(chunkCoord, localCoord);

                        VoxelData voxel;

                        if (globalCoord.Y > height)
                            voxel = Air;
                        else
                            voxel = Stone;

                        data[localCoord] = voxel;
                    }
                }
            }
        }
    }
}
