using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Storage;
using FainCraft.Gameplay.WorldScripts.Systems.Loading;
using System.Collections.Concurrent;

namespace FainCraft.Gameplay.WorldScripts.Systems.Activation;


public class RegionActivator : IRegionActivator, IDisposable
{
    private readonly IRegionDataStore _regionDataStore;
    private readonly IRegionDataLoader _regionDataLoader;
    private readonly IActiveRegionRadius _radius;
    private readonly CancellationTokenSource _cts = new();

    // Tracks desired regions: added on Load event, removed on Unload event
    private readonly ConcurrentDictionary<RegionCoord, bool> _desiredRegions = new();
    // Queue of actions to apply on main thread
    private readonly ConcurrentQueue<(RegionCoord coord, RegionData? data)> _applyQueue = new();

    public RegionActivator(IServiceProvider provider)
    {
        _regionDataStore = provider.Get<IRegionDataStore>()
            ?? throw new ArgumentNullException(nameof(provider));
        _regionDataLoader = provider.Get<IRegionDataLoader>()
            ?? throw new ArgumentNullException(nameof(provider));

        // Initialize radius with current load radius setting
        _radius = new ActiveRegionRadius(() => SharedVariables.RenderSettings.Value.LoadRadius);

        // Subscribe to radius events
        _radius.Load += OnRegionLoadEvent;
        _radius.Unload += OnRegionUnloadEvent;
    }

    /// <summary>
    /// Must be called on the main thread each tick to calculate load/unload and apply changes.
    /// </summary>
    public void Tick()
    {
        // Recalculate which regions to load/unload; will raise Load/Unload events
        _radius.Calculate();

        // Process any pending region-set operations on main thread
        while (_applyQueue.TryDequeue(out var item))
        {
            _regionDataStore.SetRegion(item.coord, item.data);
        }
    }

    private void OnRegionLoadEvent(RegionCoord coord)
    {
        // Mark region desired
        _desiredRegions[coord] = true;

        // Start async load
        _ = Task.Run(async () =>
        {
            RegionData? data = null;
            try
            {
                data = await _regionDataLoader.LoadRegionAsync(coord, _cts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // canceled, nothing to do
                return;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading region {coord}: {ex}");
                return;
            }

            // Determine if still needed
            bool stillNeeded = _desiredRegions.ContainsKey(coord);
            // UpdateMesh SetRegion on main thread: data if still needed, otherwise null
            _applyQueue.Enqueue((coord, stillNeeded ? data : null));
        }, _cts.Token);
    }

    private void OnRegionUnloadEvent(RegionCoord coord)
    {
        // Mark region not desired
        _desiredRegions.TryRemove(coord, out _);
        // UpdateMesh unload on main thread
        _applyQueue.Enqueue((coord, null));
    }

    public void Dispose()
    {
        // Cancel any in-flight loads
        _cts.Cancel();

        // Unsubscribe events
        _radius.Load -= OnRegionLoadEvent;
        _radius.Unload -= OnRegionUnloadEvent;

        _cts.Dispose();
        GC.SuppressFinalize(this);
    }

    ~RegionActivator() => Dispose();
}
