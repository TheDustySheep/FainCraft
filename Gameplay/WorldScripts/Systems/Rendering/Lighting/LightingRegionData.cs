using FainCraft.Gameplay.WorldScripts.Data;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public class LightingRegionData
    {
        const uint DOUBLE_CHUNK_AREA = 4 * CHUNK_SIZE * CHUNK_SIZE;

        readonly LightData[] _fullData = new LightData[DOUBLE_CHUNK_AREA * (VOXEL_Y_COUNT + 2)];
        readonly LightData[] _ssboData = new LightData[PADDED_REGION_VOLUME];

        RegionData[] _regions = new RegionData[9];
        public void SetRegions(IEnumerable<RegionData> regions)
        {
            _fullData.AsSpan().Clear();
            _ssboData.AsSpan().Clear();

            int i = 0;
            foreach (var region in regions)
            {
                _regions[i] = region;
                i++;
            }
        }

        public bool GetVoxel(int x, int y, int z, out VoxelState voxelState)
        {
            int r_x = x < 16 ? 0 : x < 48 ? 1 : 2;
            int r_z = z < 16 ? 0 : z < 48 ? 1 : 2;
            var region = _regions[r_x + r_z * 3];
            return region.GetVoxel((x + 16) % 32, y, (z + 16) % 32, out voxelState);
        }

        public LightData this[int x, int y, int z]
        {
            get => _fullData[x + z * 2 * CHUNK_SIZE + y * DOUBLE_CHUNK_AREA];
            set
            {
                _fullData[x + z * 2 * CHUNK_SIZE + y * DOUBLE_CHUNK_AREA] = value;
                if (x > 15 && x < 48 &&
                    z > 15 && z < 48)
                    _ssboData[(x - 16) + (z - 16) * PADDED_CHUNK_SIZE + (y) * PADDED_CHUNK_AREA] = value;
            }
        }

        // Vertical padding doesn't stack - therefore need to calculate as below
        public Span<LightData> GetSlice(int c_y)
            => _ssboData.AsSpan().Slice((c_y + REGION_Y_NEG_COUNT) * PADDED_CHUNK_AREA * CHUNK_SIZE, PADDED_CHUNK_VOLUME);
    }
}
