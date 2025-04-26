using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.HeightGenerators;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.SurfaceDecoration;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.Types
{
    internal class Biome : IBiome
    {
        public ITerrain          Terrain   { get; private set; }
        public ISurfacePainter   Painter   { get; private set; }
        public ISurfaceDecorator Decorator { get; private set; }

        public Biome(ISurfacePainter painter, ISurfaceDecorator decorator, ITerrain terrain)
        {
            Terrain = terrain;
            Painter = painter;
            Decorator = decorator;
        }

        public float SampleHeight(RegionMaps maps, VoxelCoord2DGlobal coord)
        {
            return Terrain.Generate(maps, coord);
        }
    }
}
