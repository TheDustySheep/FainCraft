using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;
public interface ITerrainGenerationSystem
{
    public Task<RegionGenerationResult> GenerateAsync(RegionCoord coord, CancellationToken token);
}
