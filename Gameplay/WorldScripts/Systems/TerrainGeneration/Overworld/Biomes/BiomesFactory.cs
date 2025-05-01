using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.HeightGenerators;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.SurfaceDecoration;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.Types;
using FainCraft.Gameplay.WorldScripts.Voxels;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes
{
    internal class BiomesFactory
    {
        readonly ISurfaceDecorator _decoratorNull;
        readonly ISurfaceDecorator _decoratorTrees;

        readonly ISurfacePainter _oceanPainter;
        readonly ISurfacePainter _hillsPainter;
        readonly ISurfacePainter _plainsPainter;
        readonly ISurfacePainter _sandyPainter;

        public readonly IBiome Ocean;
        public readonly IBiome OceanDeep;
        public readonly IBiome Plains;
        public readonly IBiome Hills;
        public readonly IBiome Desert;
        public readonly IBiome SandyShores;

        public BiomesFactory(VoxelIndexer indexer, int seed)
        {
            _decoratorNull = new NullDecorator();
            _decoratorTrees = new TreeDecorator(indexer, seed);

            _oceanPainter = new PainterOcean(indexer);
            _plainsPainter = new PainterPlains(indexer);
            _hillsPainter = new PainterHills(indexer, 70);
            _sandyPainter = new PainterSandy(indexer);

            Hills = new BiomeHills(_hillsPainter);
            Ocean = new Biome(_oceanPainter, _decoratorNull, new SimplexFBMTerrain(seed, -5f, 5f, 0.020f));
            OceanDeep = new Biome(_oceanPainter, _decoratorNull, new SimplexFBMTerrain(seed, -30f, 15f, 0.020f));
            Plains = new Biome(_plainsPainter, _decoratorTrees, new SimplexFBMTerrain(seed, 5f, 10f, 0.005f, minHeight: 1f));
            Desert = new Biome(_sandyPainter, _decoratorNull, new SimplexFBMTerrain(seed, 5f, 10f, 0.007f, minHeight: 1f));
            SandyShores = new Biome(_sandyPainter, _decoratorNull, new SimplexFBMTerrain(seed, 2f, 1f, 0.010f, minHeight: 1f));
        }
    }
}
