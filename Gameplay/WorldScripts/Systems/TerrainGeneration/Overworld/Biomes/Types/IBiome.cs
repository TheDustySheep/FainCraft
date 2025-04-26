using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.SurfaceDecoration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.Types
{
    internal interface IBiome
    {
        public ISurfacePainter   Painter { get; }
        public ISurfaceDecorator Decorator { get; }

        float SampleHeight(RegionMaps maps, VoxelCoord2DGlobal coord);
    }
}
