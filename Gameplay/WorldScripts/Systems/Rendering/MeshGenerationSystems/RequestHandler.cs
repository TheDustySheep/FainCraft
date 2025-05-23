using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Signals;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;

public class RequestHandler
{
    private readonly ISignalBus _signalBus;
    private readonly Action<ChunkCoord, bool> _updateChunk;

    public RequestHandler(ISignalBus signalBus, Action<ChunkCoord, bool> updateChunk)
    {
        _signalBus = signalBus;
        _updateChunk = updateChunk;

        _signalBus.Subscribe<LoadedRegionDataSignal>  (OnLoadedRegionData);
        _signalBus.Subscribe<ModifiedRegionDataSignal>(OnModifiedRegionData);
        _signalBus.Subscribe<ModifiedChunkDataSignal> (OnModifiedChunkData);
        _signalBus.Subscribe<ModifiedVoxelStateSignal>(OnModifiedVoxelState);
    }

    ~RequestHandler()
    {
        _signalBus.Unsubscribe<LoadedRegionDataSignal>  (OnLoadedRegionData);
        _signalBus.Unsubscribe<ModifiedRegionDataSignal>(OnModifiedRegionData);
        _signalBus.Unsubscribe<ModifiedChunkDataSignal> (OnModifiedChunkData);
        _signalBus.Unsubscribe<ModifiedVoxelStateSignal>(OnModifiedVoxelState);
    }


    private void OnLoadedRegionData(LoadedRegionDataSignal signal)
    {
        for (int dx = -1; dx < 2; dx++)
        {
            for (int dz = -1; dz < 2; dz++)
            {
                var orCoord = signal.Coord + new RegionCoord(dx, dz);

                foreach (var c_y in WorldConstants.Iterate_Y_Chunks())
                {
                    _updateChunk.Invoke(new ChunkCoord(orCoord, c_y), orCoord == signal.Coord);
                }
            }
        }
    }

    private void OnModifiedRegionData(ModifiedRegionDataSignal signal)
    {
        RegionCoord rCoord = signal.Coord;

        foreach (var c_y in WorldConstants.Iterate_Y_Chunks())
        {
            var cCoord = new ChunkCoord(rCoord, c_y);
            _updateChunk.Invoke(new ChunkCoord(rCoord, c_y), false);
        }

        foreach (var oCoord in WorldConstants.Iterate_Neighbour_Regions())
        {
            RegionCoord orCoord = rCoord + oCoord;
            foreach (var c_y in WorldConstants.Iterate_Y_Chunks())
            {
                var cCoord = new ChunkCoord(orCoord, c_y);
                _updateChunk.Invoke(new ChunkCoord(rCoord, c_y), false);
            }
        }
    }

    private void OnModifiedChunkData(ModifiedChunkDataSignal signal)
    {
        _updateChunk.Invoke(signal.Coord, false);

        foreach (var oCoord in WorldConstants.Iterate_Neighbour_Chunks())
        {
            _updateChunk.Invoke(signal.Coord, false);
        }
    }

    private void OnModifiedVoxelState(ModifiedVoxelStateSignal signal)
    {
        var cCoord = (ChunkCoord)signal.Coord;
        _updateChunk.Invoke(cCoord, false);

        foreach (var off in WorldConstants.GetTouchedNeighborOffsets((VoxelCoordLocal)signal.Coord))
        {
            var ocCoord = cCoord + off;
            _updateChunk.Invoke(ocCoord, false);
        }
    }
}
