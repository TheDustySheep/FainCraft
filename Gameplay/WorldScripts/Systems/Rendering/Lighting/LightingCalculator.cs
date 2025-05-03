using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Voxels;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public class LightingCalculator
    {
        readonly VoxelIndexer _indexer;

        public LightingCalculator(VoxelIndexer indexer)
        {
            _indexer = indexer;
        }

        public ChunkLightingData Calculate(ChunkDataCluster dataCluster, bool isTop)
        {
            bool[] solid = _indexer.CacheLightingSolid.Data;

            var data = new ChunkLightingData();

            if (!isTop)
            {
                // Uhhh TODO handle this later lol
                return data;
            }

            for (uint y = 0; y < CHUNK_SIZE; y++)
            {
                for (uint x = 0; x < CHUNK_SIZE; x++)
                {
                    for (uint z = 0; z < CHUNK_SIZE; z++)
                    {
                        data[x + 1, y + 1, z + 1] = new LightData() { Sky = (byte)y };
                    }
                }
            }

            return data;
        }
    }
}
