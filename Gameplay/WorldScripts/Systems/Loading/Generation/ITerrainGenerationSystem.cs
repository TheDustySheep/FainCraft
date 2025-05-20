using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation;
internal interface ITerrainGenerationSystem
{
    public Task<RegionGenerationResult> GenerateAsync(RegionCoord coord, CancellationToken token);
}
