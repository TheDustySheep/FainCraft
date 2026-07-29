using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Signals;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering;

public class RenderingSystem : IRenderingSystem
{
    private readonly HashSet<RegionCoord> _loaded = new();

    private readonly IMeshGenerationSystem _meshGenerator;
    private readonly IDrawingSystem _drawingSystem;
    private readonly ISignalBus _signalBus;

    public RenderingSystem(ISignalBus signalBus, IMeshGenerationSystem meshGenerator, IDrawingSystem drawingSystem)
    {
        _signalBus = signalBus;
        _meshGenerator = meshGenerator;
        _drawingSystem = drawingSystem;

        _meshGenerator.OnComplete += _drawingSystem.UpdateMesh;
        Subscribe();
    }

    ~RenderingSystem()
    {
        _meshGenerator.OnComplete -= _drawingSystem.UpdateMesh;
        Unsubscribe();
    }

    public void Tick()
    {
        _meshGenerator.Tick();
        _drawingSystem.Draw();
    }

    public void Add(RegionCoord coord)
    {
        if (_loaded.Add(coord))
        {
            _drawingSystem.Load(coord);
            _meshGenerator.Update(coord);

            foreach (var oCoord in WorldConstants.IterateNeighbourRegions())
                _meshGenerator.Update(coord + oCoord);
        }
    }

    public void Remove(RegionCoord coord)
    {
        if (_loaded.Remove(coord))
        {
            _drawingSystem.Unload(coord);
            _meshGenerator.Cancel(coord);
        }
    }

    #region Signal Handlers
    private void Subscribe()
    {
        _signalBus.Subscribe<LoadedRegionDataSignal>(OnRegionLoaded);
        _signalBus.Subscribe<UnloadedRegionDataSignal>(OnRegionUnloaded);
        _signalBus.Subscribe<ModifiedRegionDataSignal>(OnRegionUpdate);
        _signalBus.Subscribe<ModifiedChunkDataSignal>(OnChunkUpdate);
        _signalBus.Subscribe<ModifiedVoxelStateSignal>(OnVoxelUpdate);
    }

    private void Unsubscribe()
    {
        _signalBus.Unsubscribe<LoadedRegionDataSignal>(OnRegionLoaded);
        _signalBus.Unsubscribe<UnloadedRegionDataSignal>(OnRegionUnloaded);
        _signalBus.Unsubscribe<ModifiedRegionDataSignal>(OnRegionUpdate);
        _signalBus.Unsubscribe<ModifiedChunkDataSignal>(OnChunkUpdate);
        _signalBus.Unsubscribe<ModifiedVoxelStateSignal>(OnVoxelUpdate);
    }

    private void OnRegionLoaded(LoadedRegionDataSignal signal)
    {
        if (!_loaded.Contains(signal.Coord))
            return;

        _meshGenerator.Update(signal.Coord);
        foreach (var oCoord in WorldConstants.IterateNeighbourRegions())
            _meshGenerator.Update(signal.Coord + oCoord);
    }

    private void OnRegionUnloaded(UnloadedRegionDataSignal signal)
    {
        if (!_loaded.Contains(signal.Coord))
            return;

        _meshGenerator.Update(signal.Coord);
        foreach (var oCoord in WorldConstants.IterateNeighbourRegions())
            _meshGenerator.Update(signal.Coord + oCoord);
    }

    private void OnRegionUpdate(ModifiedRegionDataSignal signal)
    {
        if (!_loaded.Contains(signal.Coord))
            return;

        _meshGenerator.Update(signal.Coord);
        foreach (var oCoord in WorldConstants.IterateNeighbourRegions())
            _meshGenerator.Update(signal.Coord + oCoord);
    }

    private void OnChunkUpdate(ModifiedChunkDataSignal signal)
    {
        if (!_loaded.Contains((RegionCoord)signal.Coord))
            return;

        _meshGenerator.Update(signal.Coord);
        foreach (var oCoord in WorldConstants.IterateNeighbourChunks())
            _meshGenerator.Update(signal.Coord + oCoord);
    }

    private void OnVoxelUpdate(ModifiedVoxelStateSignal signal)
    {
        var rCoord = (RegionCoord)signal.Coord;
        if (!_loaded.Contains(rCoord))
            return;

        var cCoord = (ChunkCoord)signal.Coord;

        _meshGenerator.Update(cCoord);
        foreach (var oCoord in WorldConstants.IterateNeighbourChunks())
            _meshGenerator.Update(cCoord + oCoord);
        //foreach (var oCoord in WorldConstants.GetTouchedNeighborOffsets(signal.Coord))
        //    _meshGenerator.Update(cCoord + oCoord);
    }
    #endregion
}
