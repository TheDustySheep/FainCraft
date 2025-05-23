using FainCraft.Gameplay.WorldScripts.Coords;
using FainEngine_v2.Extensions;
using System.Collections;
using System.Runtime.CompilerServices;

namespace FainCraft.Gameplay.WorldScripts;
public static class WorldConstants
{
    public const int CHUNK_SIZE_POWER = 5;

    public const int CHUNK_SIZE_MASK = CHUNK_SIZE - 1;
    public const int CHUNK_SIZE      = 1 << CHUNK_SIZE_POWER;

    public const int CHUNK_AREA   = CHUNK_SIZE * CHUNK_SIZE;
    public const int CHUNK_VOLUME = CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE;

    public const int PADDED_CHUNK_SIZE    = CHUNK_SIZE + 2;
    public const int PADDED_CHUNK_AREA    = PADDED_CHUNK_SIZE * PADDED_CHUNK_SIZE;
    public const int PADDED_CHUNK_VOLUME  = PADDED_CHUNK_SIZE * PADDED_CHUNK_SIZE * PADDED_CHUNK_SIZE;
    public const int PADDED_REGION_VOLUME = PADDED_CHUNK_AREA * (CHUNK_SIZE * REGION_TOTAL_CHUNKS + 2);

    public const int REGION_POS_CHUNKS   = 4;
    public const int REGION_NEG_CHUNKS   = 2;
    public const int REGION_TOTAL_CHUNKS = REGION_POS_CHUNKS + REGION_NEG_CHUNKS;

    public const int REGION_VOXEL_HEIGHT = REGION_TOTAL_CHUNKS * CHUNK_SIZE;

    public const int REGION_MAX_Y_VOXEL  =  REGION_POS_CHUNKS * CHUNK_SIZE - 1;
    public const int REGION_MIN_Y_VOXEL  = -REGION_NEG_CHUNKS * CHUNK_SIZE;

    #region Enumerables
    public static IEnumerable<int> Iterate_Y_Chunks()
    {
        for (int y = -REGION_NEG_CHUNKS; y < REGION_POS_CHUNKS; y++)
            yield return y;
    }

    public static IEnumerable<RegionCoord> Iterate_Neighbour_Regions()
    {
        yield return new RegionCoord(-1, -1);
        yield return new RegionCoord( 0, -1);
        yield return new RegionCoord( 1, -1);

        yield return new RegionCoord(-1,  0);
        // Center region - don't return this one
        yield return new RegionCoord( 1,  0);

        yield return new RegionCoord(-1,  1);
        yield return new RegionCoord( 0,  1);
        yield return new RegionCoord( 1,  1);
    }

    public static IEnumerable<ChunkCoord> Iterate_Neighbour_Chunks()
    {
        for (int dy = -1; dy < 2; dy++)
        {
            for (int dz = -1; dz < 2; dz++)
            {
                for (int dx = -1; dx < 2; dx++)
                {
                    if (dx == 0 && dy == 0 && dz == 0) continue;
                    yield return new ChunkCoord(dx, dy, dz);
                }
            }
        }
    }

    public static IEnumerable<ChunkCoord> GetTouchedNeighborOffsets(VoxelCoordLocal v)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            if (dx == -1 && v.X !=  0) continue;
            if (dx ==  1 && v.X != 31) continue;

            for (int dy = -1; dy <= 1; dy++)
            {
                if (dy == -1 && v.Y !=  0) continue;
                if (dy ==  1 && v.Y != 31) continue;

                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dz == -1 && v.Z !=  0) continue;
                    if (dz ==  1 && v.Z != 31) continue;

                    if (dx == 0 && dy == 0 && dz == 0) continue;

                    yield return new ChunkCoord(dx, dy, dz);
                }
            }
        }
    }
    #endregion

    #region Indexes
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ChunkIndex(int x, int y, int z)
    {
        return
            (y & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER * 2 |
            (z & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER |
            x & CHUNK_SIZE_MASK;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ChunkIndex(uint x, uint y, uint z)
    {
        return
            (y & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER * 2 |
            (z & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER |
            x & CHUNK_SIZE_MASK;
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ClusterIndex(uint x, uint y, uint z)
    {
        return x + z * 3 + y * 9;
    }
    #endregion

    #region Position Conversions
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToGlobalFromChunk(int pos)
    {
        return pos << CHUNK_SIZE_POWER;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToChunkFromGlobal(int pos)
    {
        return pos >> CHUNK_SIZE_POWER;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToLocalFromGlobal(int pos)
    {
        return pos.Mod(CHUNK_SIZE);
    }
    #endregion

    #region Region Coord Constructors
    public static RegionCoord ConvertToRegionCoord(ChunkCoord chunkCoord)
    {
        return new RegionCoord(chunkCoord.X, chunkCoord.Z);
    }
    public static RegionCoord ConvertToRegionCoord(VoxelCoordGlobal globalCoord)
    {
        return new RegionCoord
        (
            ConvertToChunkFromGlobal(globalCoord.X),
            ConvertToChunkFromGlobal(globalCoord.Z)
        );
    }
    #endregion

    #region Global Coord Constructors    
    public static VoxelCoordGlobal ConvertToGlobalCoord(ChunkCoord chunkCoord, VoxelCoordLocal localCoord)
    {
        return new VoxelCoordGlobal()
        {
            X = ConvertToGlobalFromChunk(chunkCoord.X) + localCoord.X,
            Y = ConvertToGlobalFromChunk(chunkCoord.Y) + localCoord.Y,
            Z = ConvertToGlobalFromChunk(chunkCoord.Z) + localCoord.Z,
        };
    }

    public static VoxelCoordGlobal ConvertToGlobalCoord(VoxelCoordLocal localCoord)
    {
        return new VoxelCoordGlobal()
        {
            X = localCoord.X,
            Y = localCoord.Y,
            Z = localCoord.Z,
        };
    }

    public static VoxelCoordGlobal ConvertToGlobalCoord(RegionCoord regionCoord)
    {
        return new VoxelCoordGlobal()
        {
            X = ConvertToGlobalFromChunk(regionCoord.X),
            Y = 0,
            Z = ConvertToGlobalFromChunk(regionCoord.Z),
        };
    }

    public static VoxelCoordGlobal ConvertToGlobalCoord(ChunkCoord coord)
    {
        return new VoxelCoordGlobal(
            ConvertToGlobalFromChunk(coord.X),
            ConvertToGlobalFromChunk(coord.Y),
            ConvertToGlobalFromChunk(coord.Z));
    }
    #endregion

    #region Chunk Coord Constructors
    public static ChunkCoord ConvertToChunkCoord(VoxelCoordGlobal globalCoord)
    {
        return new ChunkCoord()
        {
            X = ConvertToChunkFromGlobal(globalCoord.X),
            Y = ConvertToChunkFromGlobal(globalCoord.Y),
            Z = ConvertToChunkFromGlobal(globalCoord.Z),
        };
    }

    public static ChunkCoord ConvertToChunkCoord(RegionCoord regionCoord)
    {
        return new ChunkCoord()
        {
            X = regionCoord.X,
            Y = 0,
            Z = regionCoord.Z,
        };
    }
    #endregion

    #region Local Coord Constructors
    public static VoxelCoordLocal ConvertToLocalCoord(VoxelCoordGlobal globalCoord)
    {
        return new VoxelCoordLocal(
            ConvertToLocalFromGlobal(globalCoord.X),
            ConvertToLocalFromGlobal(globalCoord.Y),
            ConvertToLocalFromGlobal(globalCoord.Z));
    }
    #endregion
}
