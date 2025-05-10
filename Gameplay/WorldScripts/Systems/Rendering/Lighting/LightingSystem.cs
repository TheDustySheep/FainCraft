using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
using FainEngine_v2.Collections;
using FainEngine_v2.Utils.Variables;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public class LightingSystem : ILightingSystem
    {
        private readonly IWorldData          _worldData;
        private readonly IRenderSystem       _renderSystem;
        private readonly ILightingCalculator _lightingCalculator;

        private readonly Queue<RegionCoord> _coords = new();
        private readonly HashSet<RegionCoord> _coordHash = new();

        private readonly ReferenceVariable<RenderSettings> _renderSettings;
        public LightingSystem(IWorldData worldData, IRenderSystem renderSystem, ILightingCalculator lightingCalculator)
        {
            _worldData          = worldData;
            _renderSystem       = renderSystem;
            _lightingCalculator = lightingCalculator;
            _renderSettings     = SharedVariables.RenderSettings;

            worldData.OnChunkUpdate  += OnChunkUpdate;
            renderSystem.OnMeshAdded += OnChunkUpdate;
        }

        private void OnChunkUpdate(ChunkCoord coord, bool _) => OnChunkUpdate(coord);
        private void OnChunkUpdate(ChunkCoord coord)
        {
            var rCoord = (RegionCoord)coord;
            if (_coordHash.Add(rCoord))
                _coords.Enqueue(rCoord);
        }

        LightingRegionData data = new();
        public void Tick()
        {
            for (int i = 0; i < _renderSettings.Value.LightingUpdatesPerTick; i++)
            {
                if (!_coords.TryDequeue(out var rCoord))
                    return;

                _coordHash.Remove(rCoord);

                if (!_worldData.GetRegion(rCoord, out var region5))
                    continue;

                data.SetRegions([
                    _worldData.GetRegion(rCoord + new RegionCoord(-1, -1), out var region1) ? region1 : RegionData.Empty,
                    _worldData.GetRegion(rCoord + new RegionCoord( 0, -1), out var region2) ? region2 : RegionData.Empty,
                    _worldData.GetRegion(rCoord + new RegionCoord( 1, -1), out var region3) ? region3 : RegionData.Empty,
                    _worldData.GetRegion(rCoord + new RegionCoord(-1,  0), out var region4) ? region4 : RegionData.Empty,
                    region5,                                       
                    _worldData.GetRegion(rCoord + new RegionCoord( 1,  0), out var region6) ? region6 : RegionData.Empty,
                    _worldData.GetRegion(rCoord + new RegionCoord(-1,  1), out var region7) ? region7 : RegionData.Empty,
                    _worldData.GetRegion(rCoord + new RegionCoord( 0,  1), out var region8) ? region8 : RegionData.Empty,
                    _worldData.GetRegion(rCoord + new RegionCoord( 1,  1), out var region9) ? region9 : RegionData.Empty,
                ]);

                _lightingCalculator.Calculate(data);
                _renderSystem.UpdateLighting(rCoord, data);
            }
        }
    }
}
