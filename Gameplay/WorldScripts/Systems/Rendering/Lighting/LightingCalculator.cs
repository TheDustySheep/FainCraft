using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Voxels;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public class LightingCalculator : ILightingCalculator
    {
        private readonly VoxelIndexer _indexer;
        private const byte MAX_LIGHT = 15;

        private static readonly (int dx, int dy, int dz)[] Directions =
        {
            (1,0,0), (-1,0,0), (0,1,0), (0,-1,0), (0,0,1), (0,0,-1)
        };

        public LightingCalculator(VoxelIndexer indexer)
        {
            _indexer = indexer;
        }

        public void Calculate(LightingRegionData data)
        {
            int paddedY = VOXEL_Y_COUNT + 2;
            var queue = new Queue<(int x, int y, int z)>();

            InitializeSkyLight(data, paddedY, queue);
            InitializeTorchLight(data, paddedY, queue);

            PropagateLight(data, paddedY, queue);
        }

        private void InitializeSkyLight(LightingRegionData data, int paddedY, Queue<(int x, int y, int z)> queue)
        {
            int topY = paddedY - 1;

            for (int gx = 0; gx < PADDED_CHUNK_SIZE; gx++)
                for (int gz = 0; gz < PADDED_CHUNK_SIZE; gz++)
                {
                    int wx = gx - 1;
                    int wz = gz - 1;
                    int wy = VOXEL_Y_COUNT - 1;

                    if (!data.GetVoxel(wx, wy, wz, out var voxelState)) continue;

                    var voxelType = _indexer.GetVoxelType(voxelState.Index);

                    if (!voxelType.Fully_Opaque)
                    {
                        data[topY, gx, gz].Sky = MAX_LIGHT;
                        queue.Enqueue((gx, topY, gz));
                    }
                }
        }

        private void InitializeTorchLight(LightingRegionData data, int paddedY, Queue<(int x, int y, int z)> queue)
        {
            for (int gx = 0; gx < PADDED_CHUNK_SIZE; gx++)
                for (int gy = 0; gy < paddedY; gy++)
                    for (int gz = 0; gz < PADDED_CHUNK_SIZE; gz++)
                    {
                        int wx = gx - 1;
                        int wy = gy - 1;
                        int wz = gz - 1;

                        if (!data.GetVoxel(wx, wy, wz, out var voxelState)) continue;

                        var voxelType = _indexer.GetVoxelType(voxelState.Index);

                        if (voxelType.Emits_Light > 0)
                        {
                            data[gx, gy, gz].Torch = voxelType.Emits_Light;
                            queue.Enqueue((gx, gy, gz));
                        }
                    }
        }

        private void PropagateLight(LightingRegionData data, int paddedY, Queue<(int x, int y, int z)> queue)
        {
            while (queue.Count > 0)
            {
                var (x, y, z) = queue.Dequeue();
                var currentLight = data[x, y, z];

                foreach (var (dx, dy, dz) in Directions)
                {
                    int nx = x + dx, ny = y + dy, nz = z + dz;

                    if (nx < 0 || nx >= PADDED_CHUNK_SIZE || ny < 0 || ny >= paddedY || nz < 0 || nz >= PADDED_CHUNK_SIZE)
                        continue;

                    if (!data.GetVoxel(nx - 1, ny - 1, nz - 1, out var neighborState)) continue;
                    var neighborType = _indexer.GetVoxelType(neighborState.Index);

                    // Skip opaque neighbors
                    if (neighborType.Fully_Opaque) continue;

                    ref var neighborLight = ref data[nx, ny, nz];

                    bool updated = false;

                    if (currentLight.Sky > 0)
                    {
                        byte newSky = (byte)(currentLight.Sky - 1);
                        if (newSky > neighborLight.Sky)
                        {
                            neighborLight.Sky = newSky;
                            updated = true;
                        }
                    }

                    if (currentLight.Torch > 0)
                    {
                        byte newTorch = (byte)(currentLight.Torch - 1);
                        if (newTorch > neighborLight.Torch)
                        {
                            neighborLight.Torch = newTorch;
                            updated = true;
                        }
                    }

                    if (updated)
                        queue.Enqueue((nx, ny, nz));
                }
            }
        }
    }
}