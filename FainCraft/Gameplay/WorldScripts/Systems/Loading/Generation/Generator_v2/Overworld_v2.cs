using FainCraft.Gameplay.OldWorldScripts.Editing;
using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainEngine_v2.Utils;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v2;

internal class Overworld_v2 : ITerrainGenerator
{
    private readonly RegionEditList _regionEditList = new();
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
        ApplyEdits(rCoord, data);
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
                        var coord = rvCoord + new VoxelCoordLocal(x, y, z);
                        var local = (VoxelCoordLocal)coord;
                        data.SetVoxel(local.X, local.Y+64, local.Z, _indexer.GetVoxel("Log"));
                        //AddTree(coord, s);
                        break;
                    }
                }
            }
        }
    }

    private void AddTree(VoxelCoordGlobal gvCoord, uint s)
    {
        var log = _indexer.GetVoxel("Log");
        var leaves = _indexer.GetVoxel("Leaves");

        int height = (int)s & 0b111;

        for (int l_y = 0; l_y < height; l_y++)
        {
            _regionEditList.AddEdit(
                gvCoord + new VoxelCoordLocal(0, l_y, 0),
                new VoxelEditReplaceTarget(log, default)
            );
        }

        for (int l_x = -2; l_x < 3; l_x++)
        {
            for (int l_z = -2; l_z < 3; l_z++)
            {
                for (int l_y = -2; l_y < 3; l_y++)
                {
                    _regionEditList.AddEdit(
                        gvCoord + new VoxelCoordGlobal(l_x, l_y + height, l_z), 
                        new VoxelEditReplaceTarget(leaves, default)
                    );
                }
            }
        }
    }

    private void ApplyEdits(RegionCoord coord, RegionData data)
    {
        _regionEditList.ApplyEdits(coord , data);
    }
}
