using FainCraft.Gameplay.WorldScripts.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.HeightGenerators
{
    internal interface ITerrain
    {
        float Generate(RegionMaps maps, VoxelCoord2DGlobal coord);
    }
}
