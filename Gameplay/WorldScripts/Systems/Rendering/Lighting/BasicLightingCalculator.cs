using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Voxels;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public class BasicLightingCalculator : ILightingCalculator
    {
        readonly VoxelIndexer _indexer;
        const int MAX_LIGHT = 31;
        static readonly (int dx, int dy, int dz)[] Directions = new[]
        {
        (1, 0, 0), (-1, 0, 0),
        (0, 0, 1), (0, 0, -1),
        (0, 1, 0), (0, -1, 0)
    };

        public BasicLightingCalculator(VoxelIndexer indexer)
        {
            _indexer = indexer;
        }

        public void Calculate(LightingRegionData data)
        {
            bool[] isSolid = _indexer.CacheLightPassThrough.Data;

            Queue<(int x, int y, int z)> queue = new Queue<(int x, int y, int z)>();

            // STEP 1: Initialize skylight vertically
            for (int l_x = 0; l_x < CHUNK_SIZE * 2; l_x++)
            {
                for (int l_z = 0; l_z < CHUNK_SIZE * 2; l_z++)
                {
                    byte sky = MAX_LIGHT;
                    for (int i_y = VOXEL_Y_COUNT - 1; i_y >= 0; i_y--)
                    {
                        if (!data.GetVoxel(l_x, i_y, l_z, out var voxelState))
                            continue;

                        VoxelType voxelType = _indexer.GetVoxelType(voxelState.Index);

                        if (voxelType.Physics_Solid)
                        {
                            sky = 0;
                        }

                        data[l_x + 1, i_y + 1, l_z + 1] = new LightData { Sky = sky };

                        if (sky > 0)
                        {
                            queue.Enqueue((l_x + 1, i_y + 1, l_z + 1));
                        }
                    }
                }
            }

            // STEP 2: Propagate skylight horizontally (but no vertical attenuation downward)
            while (queue.Count > 0)
            {
                var (x, y, z) = queue.Dequeue();
                byte currentLight = data[x, y, z].Sky;

                foreach (var (dx, dy, dz) in Directions)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    int nz = z + dz;

                    // skip out of bounds (regionData assumed padded +1)
                    if (nx < 1 || nx >= CHUNK_SIZE * 2 + 1 ||
                        ny < 1 || ny >= VOXEL_Y_COUNT + 1 ||
                        nz < 1 || nz >= CHUNK_SIZE * 2 + 1)
                        continue;

                    if (!data.GetVoxel(nx - 1, ny - 1, nz - 1, out var neighborState))
                        continue;

                    VoxelType neighborType = _indexer.GetVoxelType(neighborState.Index);
                    bool blocked = neighborType.Physics_Solid;

                    byte neighborLight = data[nx, ny, nz].Sky;
                    byte propagatedLight = (byte)(currentLight - (dx == 0 && dz == 0 ? 0 : 1)); // no vertical attenuation

                    if (propagatedLight > 0 && !blocked && propagatedLight > neighborLight)
                    {
                        data[nx, ny, nz] = new LightData { Sky = propagatedLight };
                        queue.Enqueue((nx, ny, nz));
                    }
                }
            }
        }
    }
}
