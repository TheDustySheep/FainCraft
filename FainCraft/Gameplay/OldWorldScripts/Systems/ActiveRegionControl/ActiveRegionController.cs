using FainCraft.Gameplay.OldWorldScripts.Systems.RegionLoading;
using FainCraft.Gameplay.WorldScripts.Coords;
using Silk.NET.Maths;

namespace FainCraft.Gameplay.OldWorldScripts.Systems.ActiveRegionControl;

internal class ActiveRegionController : IActiveRegionController
{
    readonly IRegionLoadingController loadingController;
    readonly HashSet<RegionCoord> ActiveRegions = new();
    bool hasInit = false;

    int LOAD_RADIUS = 20;

    public ActiveRegionController(IRegionLoadingController loadingController)
    {
        this.loadingController = loadingController;
    }

    public void Tick()
    {
        if (hasInit)
            return;

        foreach (var point in GetSpiral(LOAD_RADIUS))
        {
            var coord = new RegionCoord(point.X, point.Y);
            ActiveRegions.Add(coord);
            loadingController.Load(coord);
        }

        hasInit = true;
    }

    static IEnumerable<Vector2D<int>> GetSpiral(int radius)
    {
        var point = new Vector2D<int>(0, 0);

        yield return point;
        int sign = 1;
        for (int row = 1; row < radius * 2; row++)
        {
            // move right/left by row, and then up/down by row
            for (int k = 0; k < row; k++)
            {
                point += new Vector2D<int>(sign * 1, 0);
                yield return point;
            }
            for (int k = 0; k < row; k++)
            {
                point += new Vector2D<int>(0, -sign * 1);
                yield return point;
            }
            sign *= -1;
        }
        // last leg to finish filling the area
        for (int k = 0; k < (radius * 2) - 1; k++)
        {
            point += new Vector2D<int>(sign * 1, 0);
            yield return point;
        }

    }
}
