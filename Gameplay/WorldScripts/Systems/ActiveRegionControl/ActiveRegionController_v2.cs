using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering;
using FainEngine_v2.Utils.Variables;

namespace FainCraft.Gameplay.WorldScripts.Systems.ActiveRegionControl
{
    public class ActiveRegionController_v2 : IActiveRegionController
    {
        RegionCoord?  _lastCoord;

        readonly HashSet<RegionCoord> ActiveRegions = new();

        public event Action<RegionCoord>? OnLoad;
        public event Action<RegionCoord>? OnUnload;

        readonly ReferenceVariable<PlayerPosition> _playerPosition;
        readonly ReferenceVariable<RenderSettings> _renderSettings;

        public ActiveRegionController_v2()
        {
            _playerPosition = SharedVariables.PlayerPosition;
            _renderSettings = SharedVariables.RenderSettings;
        }

        public void Tick()
        {
            var playerCoord = _playerPosition.Value.RegionCoord;
            if (_lastCoord == playerCoord) return;
            _lastCoord = playerCoord;

            // materialize to lock in the spiral order
            var spiral = GetSpiral(playerCoord, _renderSettings.Value.LoadRadius)
                            .ToList();

            // 1) Unload anything no longer in the spiral
            var toUnload = ActiveRegions.Except(spiral).ToList();
            foreach (var r in toUnload)
            {
                ActiveRegions.Remove(r);
                OnUnload?.Invoke(r);
            }

            // 2) Walk the spiral in order; Add returns true only for brand-new items
            foreach (var r in spiral)
            {
                if (ActiveRegions.Add(r))
                    OnLoad?.Invoke(r);
            }
        }

        private static IEnumerable<RegionCoord> GetSpiral(RegionCoord player, int radius)
        {
            var point = player;

            yield return point;

            int sign = 1;
            for (int row = 1; row < radius * 2; row++)
            {
                // move right/left by row, and then up/down by row
                for (int k = 0; k < row; k++)
                {
                    point += new RegionCoord(sign * 1, 0);
                    yield return point;
                }
                for (int k = 0; k < row; k++)
                {
                    point += new RegionCoord(0, -sign * 1);
                    yield return point;
                }
                sign *= -1;
            }
            // last leg to finish filling the area
            for (int k = 0; k < (radius * 2) - 1; k++)
            {
                point += new RegionCoord(sign * 1, 0);
                yield return point;
            }

        }
    }
}
