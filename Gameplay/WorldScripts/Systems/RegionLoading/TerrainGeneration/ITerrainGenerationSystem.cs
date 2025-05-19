using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionLoading.TerrainGeneration;
internal interface ITerrainGenerationSystem
{
    public Task<RegionGenerationResult?> GenerateAsync(RegionCoord coord, CancellationToken token);
}
