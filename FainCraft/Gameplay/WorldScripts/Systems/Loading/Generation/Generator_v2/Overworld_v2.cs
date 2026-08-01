using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v2;

internal class Overworld_v2 : ITerrainGenerator
{
    private readonly IVoxelIndexer _indexer;
    private readonly FastNoiseLite _terrainNoise;
    public Overworld_v2(IVoxelIndexer indexer)
    {
        _indexer = indexer;

        _terrainNoise = new();
        _terrainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        _terrainNoise.SetFrequency(0.01f);
        _terrainNoise.SetFractalOctaves(2);
    }

    public RegionData Generate(RegionCoord rCoord)
    {
        RegionData data = new();
        GenerateLayers(rCoord, data);
        GenerateTrees(rCoord, data);
        return data;
    }

    private void GenerateLayers(RegionCoord rCoord, RegionData data)
    {
        VoxelCoordGlobal rvCoord = (VoxelCoordGlobal)rCoord;

        for (int x = 0; x < WorldConstants.CHUNK_SIZE; x++)
        {
            for (int z = 0; z < WorldConstants.CHUNK_SIZE; z++)
            {
                float height = _terrainNoise.GetNoise(rvCoord.X + x, rvCoord.Z + z);
                int levelHeight = (int)(64f + height * 20f);
                for (int y = WorldConstants.REGION_MIN_Y_VOXEL; y < WorldConstants.REGION_MAX_Y_VOXEL; y++)
                {
                    VoxelState state = default;
                    if (y == levelHeight)
                        state = _indexer.GetVoxel("Grass");
                    else if (y < levelHeight)
                        state = _indexer.GetVoxel("Stone");

                    data.SetVoxel(x, y, z, state);
                }
            }
        }
    }

    private void GenerateTrees(RegionCoord rCoord, RegionData data)
    {
        VoxelCoordGlobal rvCoord = (VoxelCoordGlobal)rCoord;

        uint s = (uint)rCoord.GetHashCode();

        for (int x = 0; x < WorldConstants.CHUNK_SIZE; x++)
        {
            for (int z = 0; z < WorldConstants.CHUNK_SIZE; z++)
            {
                s ^= s << 13;
                s ^= s >> 17;
                s ^= s << 5;
                float val = s / (float)uint.MaxValue;

                if (val < 0.98f)
                    continue;

                for (int y = WorldConstants.REGION_MAX_Y_VOXEL - 1; y > WorldConstants.REGION_MIN_Y_VOXEL; y--)
                {
                    if (!data.GetVoxel(x, y - 1, z, out var ground))
                        break;

                    if (ground == _indexer.GetVoxel("Grass"))
                    {
                        AddTree(x, y, z, s, data);
                        break;
                    }

                }
            }
        }
    }

    private void AddTree(int x, int y, int z, uint s, RegionData data)
    {
        var log = _indexer.GetVoxel("Log");
        var leaves = _indexer.GetVoxel("Leaves");

        int height = (int)s & 0b111;

        for (int l_x = -2; l_x < 3; l_x++)
        {
            for (int l_z = -2; l_z < 3; l_z++)
            {
                for (int l_y = -2; l_y < 3; l_y++)
                {
                    data.SetVoxel(l_x + x, y + l_y + height, l_z + z, leaves);
                }
            }
        }

        for (int l_y = 0; l_y < height; l_y++)
        {
            data.SetVoxel(x, y + l_y, z, log);
        }
    }
}
