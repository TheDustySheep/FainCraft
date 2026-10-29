using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.ChunkDataArrays;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using System.Linq.Expressions;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Data.Chunks;
public class ChunkDataRef
{
    private uint _versionID;

    private ReaderWriterLockSlim _rwLock = new(LockRecursionPolicy.NoRecursion);
    private IChunkDataStorage _storage;

    public ChunkDataRef(IChunkDataStorage storage)
    {
        _storage = storage;
    }

    #region Reading
    public uint VersionID
    {
        get
        {
            _rwLock.EnterReadLock();
            try
            {
                return _versionID;
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }
    }

    public bool IsEmpty()
    {
        _rwLock.EnterReadLock();
        try
        {
            return _storage.IsEmpty();
        }
        finally
        {
            _rwLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Copies the underlying voxel data.
    /// </summary>
    /// <param name="destination">Destination span for voxel data</param>
    /// <returns>Version ID</returns>
    public uint CopyTo(Span<VoxelState> destination)
    {
        _rwLock.EnterReadLock();
        try
        {
            _storage.CopyTo(destination);
            return _versionID;
        }
        finally
        {
            _rwLock.ExitReadLock();
        }
    }

    private VoxelState Get(uint index)
    {
        _rwLock.EnterReadLock();
        try
        {
            return _storage.Get(index);
        }
        finally
        {
            _rwLock.ExitReadLock();
        }
    }
    #endregion

    #region Writing
    private void Set(uint index, VoxelState state)
    {
        _rwLock.EnterWriteLock();
        try
        {
            _storage.Set(index, state);
            _versionID++;
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }
    #endregion

    #region Accessors
    public VoxelState this[int index]
    {
        get => Get((uint)index);
        set => Set((uint)index, value);
    }

    public VoxelState this[uint index]
    {
        get => Get(index);
        set => Set(index, value);
    }

    public VoxelState this[int x, int y, int z]
    {
        get => Get((uint)ChunkIndex(x, y, z));
        set => Set((uint)ChunkIndex(x, y, z), value);
    }

    public VoxelState this[uint x, uint y, uint z]
    {
        get => Get(ChunkIndex(x, y, z));
        set => Set(ChunkIndex(x, y, z), value);
    }

    public VoxelState this[VoxelCoordLocal localVoxelCoord]
    {
        get => Get(localVoxelCoord.VoxelIndex);
        set => Set(localVoxelCoord.VoxelIndex, value);
    }
    #endregion

    ~ChunkDataRef()
    {
        _rwLock?.Dispose();
    }
}