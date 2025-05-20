using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using FainCraft.Gameplay.WorldScripts.Signals;
using FainEngine_v2.Utils;
namespace FainCraft.Gameplay.WorldScripts.Storage;

public class VoxelDataStore : IVoxelDataStore
{
    IEventBus _eventBus;
    IChunkDataStore _chunkStore;

    public VoxelDataStore(IServiceProvider serviceProvider)
    {
        _eventBus   = serviceProvider.Get<IEventBus>();
        _chunkStore = serviceProvider.Get<IChunkDataStore>();
    }

    public bool GetVoxelState(VoxelCoordGlobal coord, out VoxelState state)
    {
        state = default;
        if (!InBounds(coord))
            return false;

        ChunkCoord cCoord = (ChunkCoord)coord;
        if (!_chunkStore.GetChunkData(cCoord, out var chunkData))
            return false;

        state = chunkData[(VoxelCoordLocal)coord];
        return true;
    }

    public bool SetVoxelState(VoxelCoordGlobal coord, VoxelState newState)
    {
        if (!InBounds(coord))
            return false;

        ChunkCoord cCoord = (ChunkCoord)coord;
        if (!_chunkStore.GetChunkData(cCoord, out var chunkData))
            return false;

        var localCoord = (VoxelCoordLocal)coord;
        var oldState = chunkData[localCoord];

        if (oldState != newState)
        {
            chunkData[localCoord] = newState;
            _eventBus.Publish(new ModifiedVoxelStateSignal()
            {
                Coord = coord,
                OldState = oldState,
                NewState = newState,
            });
        }

        return true;
    }

    public bool EditVoxelState(VoxelCoordGlobal coord, Func<VoxelState, VoxelState> editFunc)
    {
        if (!InBounds(coord))
            return false;

        ChunkCoord cCoord = (ChunkCoord)coord;
        if (!_chunkStore.GetChunkData(cCoord, out var chunkData))
            return false;

        var localCoord = (VoxelCoordLocal)coord;
        var oldState = chunkData[localCoord];
        var newState = editFunc.Invoke(oldState);

        if (oldState != newState)
        {
            chunkData[localCoord] = newState;
            _eventBus.Publish(new ModifiedVoxelStateSignal()
            {
                Coord = coord,
                OldState = oldState,
                NewState = newState,
            });
        }

        return true;
    }

    public async Task<VoxelState?> GetVoxelStateAsync(VoxelCoordGlobal coord, CancellationToken token)
    {
        if (!InBounds(coord))
            return null;

        ChunkCoord cCoord = (ChunkCoord)coord;
        var chunkData = await _chunkStore.GetChunkDataAsync(cCoord, token);
        await MainThreadDispatcher.Yield();

        // Chunk is outside bounds? Should never actually fire
        if (chunkData == null)
            return null;

        return chunkData[(VoxelCoordLocal)coord];
    }

    private static bool InBounds(VoxelCoordGlobal coord) =>
        coord.Y >= WorldConstants.REGION_MIN_Y_VOXEL &&
        coord.Y <= WorldConstants.REGION_MAX_Y_VOXEL;
}
