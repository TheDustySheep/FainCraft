using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v1;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v1.Biomes.HeightGenerators;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v1.Biomes.SurfaceDecoration;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v1.Biomes.SurfacePainting;

namespace FainCraft.Gameplay.WorldScripts.Systems.Loading.Generation.Generator_v1.Biomes.Types
{
    internal class Biome : IBiome
    {
        public ITerrain Terrain { get; private set; }
        public ISurfacePainter Painter { get; private set; }
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
