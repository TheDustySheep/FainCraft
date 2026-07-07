using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Signals;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;

public class RequestHandler
{
    private readonly ISignalBus _signalBus;
    private readonly Action<ChunkCoord, bool> _updateChunk;
    private readonly Action<RegionCoord, bool> _updateRegion;

    public RequestHandler(
        ISignalBus signalBus, 
        Action<ChunkCoord, bool> updateChunk,
        Action<RegionCoord, bool> updateRegion)
    {
        _signalBus    = signalBus;
        _updateChunk  = updateChunk;
        _updateRegion = updateRegion;

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
        _updateRegion.Invoke(signal.Coord, true);

        foreach (var rOffset in WorldConstants.Iterate_Neighbour_Regions())
            _updateRegion.Invoke(signal.Coord + rOffset, false);
    }

    private void OnModifiedRegionData(ModifiedRegionDataSignal signal)
    {
        _updateRegion.Invoke(signal.Coord, false);

        foreach (var rOffset in WorldConstants.Iterate_Neighbour_Regions())
            _updateRegion.Invoke(signal.Coord + rOffset, false);
    }

    private void OnModifiedChunkData(ModifiedChunkDataSignal signal)
    {
        _updateChunk.Invoke(signal.Coord, false);

        foreach (var oCoord in WorldConstants.Iterate_Neighbour_Chunks())
            _updateChunk.Invoke(signal.Coord, false);
    }

    private void OnModifiedVoxelState(ModifiedVoxelStateSignal signal)
    {
        var cCoord = (ChunkCoord)signal.Coord;
        _updateChunk.Invoke(cCoord, false);

        foreach (var off in WorldConstants.GetTouchedNeighborOffsets(signal.Coord))
            _updateChunk.Invoke(cCoord + off, false);
    }
}
