using FainCraft.Gameplay.OldWorldScripts.Editing;
using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Regions;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Overworld.Biomes.SurfaceDecoration
{
    internal class NullDecorator : ISurfaceDecorator
    {
        public void HandleSpawn(RegionEditList edits, RegionData regionData, RegionCoord regionCoord) { }
    }
}
