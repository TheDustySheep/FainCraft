using FainCraft.Gameplay.WorldScripts.Core;
using FainEngine_v2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.HeightMaps
{
    public class HeightMapGenerator
    {
        readonly Array2D<float> _ridges     = new(CHUNK_SIZE, CHUNK_SIZE);
        readonly Array2D<float> _continents = new(CHUNK_SIZE, CHUNK_SIZE);

        readonly RidgeGenerator     _ridgeGenerator;
        readonly ContinentGenerator _continentGenerator;

        public HeightMapGenerator(int seed)
        {
            _ridgeGenerator     = new RidgeGenerator(seed);
            _continentGenerator = new ContinentGenerator(seed);
        }

        public void Generate(RegionCoord regionCoord)
        {
            _ridgeGenerator    .Generate(_ridges, regionCoord);
            _continentGenerator.Generate(_continents, regionCoord);
        }

        public int GetHeight(int x, int y)
        {
            float ridge     = _ridges[x, y];
            float continent = _continents[x, y];

            // Only allow ridges on continents
            ridge *= continent;

            return (int)(ridge * 96);
        }
    }
}
