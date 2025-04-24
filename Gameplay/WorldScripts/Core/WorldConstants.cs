using FainEngine_v2.Extensions;
using System.Runtime.CompilerServices;

namespace FainCraft.Gameplay.WorldScripts.Core;
public class WorldConstants
{
    public const int CHUNK_SIZE_POWER = 5;

    public const int CHUNK_SIZE_MASK = CHUNK_SIZE - 1;
    public const int CHUNK_SIZE = 1 << CHUNK_SIZE_POWER;
    public const int CHUNK_SIZE_PAD = CHUNK_SIZE + 2;

    public const int CHUNK_AREA = CHUNK_SIZE * CHUNK_SIZE;
    public const int CHUNK_VOLUME = CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE;

    public const int REGION_Y_POS_COUNT = 4;
    public const int REGION_Y_NEG_COUNT = 2;
    public const int REGION_Y_TOTAL_COUNT = REGION_Y_POS_COUNT + REGION_Y_NEG_COUNT;

    public const int VOXEL_MAX_Y = REGION_Y_POS_COUNT * CHUNK_SIZE - 1;
    public const int VOXEL_MIN_Y = -REGION_Y_NEG_COUNT * CHUNK_SIZE;

    #region Indexes
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ChunkIndexOld(int x, int y, int z)
    {
        return z * CHUNK_SIZE * CHUNK_SIZE + y * CHUNK_SIZE + x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ChunkIndexOld(uint x, uint y, uint z)
    {
        return z * CHUNK_SIZE * CHUNK_SIZE + y * CHUNK_SIZE + x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ChunkIndex(int x, int y, int z)
    {
        return
            (z & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER * 2 |
            (y & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER |
            (x & CHUNK_SIZE_MASK);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ChunkIndex(uint x, uint y, uint z)
    {
        return
            (z & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER * 2 |
            (y & CHUNK_SIZE_MASK) << CHUNK_SIZE_POWER |
            (x & CHUNK_SIZE_MASK);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ClusterIndex(uint x, uint y, uint z)
    {
        return z * 9 + y * 3 + x;
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
    public static RegionCoord ConvertToRegionCoord(GlobalVoxelCoord globalCoord)
    {
        return new RegionCoord
        (
            ConvertToChunkFromGlobal(globalCoord.X),
            ConvertToChunkFromGlobal(globalCoord.Z)
        );
    }
    #endregion

    #region Global Coord Constructors    
    public static GlobalVoxelCoord ConvertToGlobalCoord(ChunkCoord chunkCoord, LocalVoxelCoord localCoord)
    {
        return new GlobalVoxelCoord()
        {
            X = ConvertToGlobalFromChunk(chunkCoord.X) + localCoord.X,
            Y = ConvertToGlobalFromChunk(chunkCoord.Y) + localCoord.Y,
            Z = ConvertToGlobalFromChunk(chunkCoord.Z) + localCoord.Z,
        };
    }

    public static GlobalVoxelCoord ConvertToGlobalCoord(LocalVoxelCoord localCoord)
    {
        return new GlobalVoxelCoord()
        {
            X = localCoord.X,
            Y = localCoord.Y,
            Z = localCoord.Z,
        };
    }

    public static GlobalVoxelCoord ConvertToGlobalCoord(RegionCoord regionCoord)
    {
        return new GlobalVoxelCoord()
        {
            X = ConvertToGlobalFromChunk(regionCoord.X),
            Y = 0,
            Z = ConvertToGlobalFromChunk(regionCoord.Z),
        };
    }

    public static GlobalVoxelCoord ConvertToGlobalCoord(ChunkCoord coord)
    {
        return new GlobalVoxelCoord(
            ConvertToGlobalFromChunk(coord.X),
            ConvertToGlobalFromChunk(coord.Y),
            ConvertToGlobalFromChunk(coord.Z));
    }
    #endregion

    #region Chunk Coord Constructors
    public static ChunkCoord ConvertToChunkCoord(GlobalVoxelCoord globalCoord)
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
    public static LocalVoxelCoord ConvertToLocalCoord(GlobalVoxelCoord globalCoord)
    {
        return new LocalVoxelCoord(
            ConvertToLocalFromGlobal(globalCoord.X),
            ConvertToLocalFromGlobal(globalCoord.Y),
            ConvertToLocalFromGlobal(globalCoord.Z));
    }
    #endregion
}
