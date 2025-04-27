using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Editing;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.SurfaceDecoration
{
    internal class NullDecorator : ISurfaceDecorator
    {
        public void HandleSpawn(RegionEditList edits, RegionData regionData, RegionCoord regionCoord) { }
    }
}
