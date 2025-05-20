using FainCraft.Gameplay.PlayerScripts;
using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.RenderSystems;
using FainEngine_v2.Utils.Variables;

namespace FainCraft.Gameplay.WorldScripts.Systems;

public class ActiveRegionRadius : IActiveRegionRadius
{
    private readonly ReferenceVariable<PlayerPosition> _playerPosition;
    private readonly Func<uint> _radiusFunc;

    public event Action<RegionCoord>? Load;
    public event Action<RegionCoord>? Unload;

    private RegionCoord? _lastCoord;

    private readonly HashSet<RegionCoord> ActiveRegions = new();

    public ActiveRegionRadius(Func<uint> radiusFunc)
    {
        _radiusFunc = radiusFunc;
        _playerPosition = SharedVariables.PlayerPosition;
    }

    public void Tick()
    {
        var playerCoord = _playerPosition.Value.RegionCoord;
        if (_lastCoord == playerCoord) return;
        _lastCoord = playerCoord;

        // materialize to lock in the spiral order
        var spiral = GetSpiral(playerCoord, _radiusFunc.Invoke());

        var toUnload = ActiveRegions.Except(spiral);
        foreach (var r in toUnload)
        {
            ActiveRegions.Remove(r);
            Unload?.Invoke(r);
        }

        foreach (var r in spiral)
        {
            if (ActiveRegions.Add(r))
                Load?.Invoke(r);
        }
    }

    private static IEnumerable<RegionCoord> GetSpiral(RegionCoord player, uint radius)
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
