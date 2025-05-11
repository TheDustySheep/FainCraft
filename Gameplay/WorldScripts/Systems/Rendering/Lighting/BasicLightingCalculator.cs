using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Voxels;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public class BasicLightingCalculator : ILightingCalculator
    {
        private readonly VoxelIndexer _indexer;
        private const byte MAX_LIGHT = 15;

        public BasicLightingCalculator(VoxelIndexer indexer)
        {
            _indexer = indexer;
        }

        public void Calculate(LightingRegionData data)
        {
            bool[] lightPass  = _indexer.CacheLightPassThrough.Data;
            byte[] emitsLight = _indexer.CacheEmitsLight.Data;

            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    byte sky = MAX_LIGHT;
                    for (int y = REGION_VOXEL_HEIGHT; y >= 1; y--)
                    {
                        data.GetVoxel(x, y, z, out var voxelState);

                        if (!lightPass[voxelState.Index])
                            break;

                        data[x, y, z] = new LightData()
                        {
                            Sky = sky,
                        };
                    }
                }
            }
        }
    }
}