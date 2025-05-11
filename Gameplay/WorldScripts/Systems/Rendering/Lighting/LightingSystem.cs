using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
using FainEngine_v2.Collections;
using FainEngine_v2.Utils;
using FainEngine_v2.Utils.Variables;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public class LightingSystem : ILightingSystem
    {
        private readonly IWorldData          _worldData;
        private readonly IRenderSystem       _renderSystem;
        private readonly ILightingCalculator _lightingCalculator;

        private readonly LightQueue _lightQueue = new();

        private readonly ReferenceVariable<RenderSettings> _renderSettings;
        private readonly ReferenceVariable<PlayerPosition> _playerPosition;

        private readonly WorkerThread workerThread;

        public LightingSystem(IWorldData worldData, IRenderSystem renderSystem, ILightingCalculator lightingCalculator)
        {
            _worldData          = worldData;
            _renderSystem       = renderSystem;
            _lightingCalculator = lightingCalculator;

            _renderSettings     = SharedVariables.RenderSettings;
            _playerPosition     = SharedVariables.PlayerPosition;

            worldData.OnChunkUpdate  += OnChunkUpdate;
            renderSystem.OnMeshAdded += OnChunkUpdate;

            workerThread = new WorkerThread("Lighting Thread", WorkerThreadFunction);
        }

        private void OnChunkUpdate(ChunkCoord coord, bool _) => OnChunkUpdate(coord);
        private void OnChunkUpdate(ChunkCoord coord)
        {
            var rCoord = (RegionCoord)coord;

            if (!_worldData.GetRegion(rCoord, out var region5))
                return;

            _lightQueue.EnqueueRequest(rCoord, [
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
        }

        private void WorkerThreadFunction()
        {
            _lightQueue.ProcessInBuffer(_playerPosition.Value.RegionCoord);

            while (true)
            {
                if (!_lightQueue.TryRequestLightingData(out var genData))
                    break;

                if (!_lightQueue.DequeueRequest(out RegionCoord regionCoord, out RegionData[] regionDatas))
                {
                    _lightQueue.ReturnLightingData(genData);
                    break;
                }

                genData.SetRegions(regionDatas);
                _lightingCalculator.Calculate(genData);
                _lightQueue.EnqueueComplete(regionCoord, genData);
            }
        }

        public void Tick()
        {
            for (int i = 0; i < _renderSettings.Value.LightingUpdatesPerTick; i++)
            {
                if (!_lightQueue.TryDequeueComplete(out var coord, out var data))
                    return;

                _renderSystem.UpdateLighting(coord, data);
                _lightQueue.ReturnLightingData(data);
            }
        }
    }
}
