using FainCraft.Gameplay.WorldScripts.Data;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public class LightingRegionData
    {
        readonly LightData[] _ssboData = new LightData[PADDED_REGION_VOLUME];

        RegionData[] _regions = new RegionData[9];
        public void SetRegions(IEnumerable<RegionData> regions)
        {
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
            return _regions[4].GetVoxel(x, y, z, out voxelState);
        }

        public ref LightData this[int x, int y, int z]
        {
            get => ref _ssboData[x + z * PADDED_CHUNK_SIZE + y * PADDED_CHUNK_AREA];
        }

        // Vertical padding doesn't stack - therefore need to calculate as below
        public Span<LightData> GetSlice(int c_y)
            => _ssboData.AsSpan().Slice((c_y + REGION_Y_NEG_COUNT) * PADDED_CHUNK_AREA * CHUNK_SIZE, PADDED_CHUNK_VOLUME);
    }
}
