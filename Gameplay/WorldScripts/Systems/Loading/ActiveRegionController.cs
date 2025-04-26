using FainCraft.Gameplay.WorldScripts.Core;
using Silk.NET.Maths;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading;

internal class ActiveRegionController : IActiveRegionController
{
    readonly ILoadingController loadingController;
    readonly HashSet<RegionCoord> ActiveRegions = new();
    bool hasInit = false;

    int LOAD_RADIUS = 16;

    public ActiveRegionController(ILoadingController loadingController)
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
            loadingController.OnLoad(coord);
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
