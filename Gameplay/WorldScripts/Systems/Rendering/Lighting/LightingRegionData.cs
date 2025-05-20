using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting
{
    public class LightingRegionData
    {
        public readonly Queue<(int, int, int)> TorchQueue = new();
        public readonly Queue<(int, int, int)> SkyQueue   = new();

        private const int CHUNK_SIZE = 32;
        private const int REGION_CHUNKS_Y = 6;
        private const int REGION_HEIGHT = CHUNK_SIZE * REGION_CHUNKS_Y;

        // Padding sizes
        private const int SSBO_PADDING = 1;              // SSBO needs 1-voxel border
        private const int CALC_PADDING_XZ = 15;           // allows ±15 in X and Z
        private const int CALC_PADDING_Y = 1;             // only need ±1 in Y

        // Buffer dimensions (flat packed X->Z->Y)
        private static readonly int SSBO_SIZE_X = CHUNK_SIZE + SSBO_PADDING * 2;
        private static readonly int SSBO_SIZE_Z = CHUNK_SIZE + SSBO_PADDING * 2;
        private static readonly int SSBO_SIZE_Y = REGION_HEIGHT + SSBO_PADDING * 2;

        private static readonly int CALC_SIZE_X = CHUNK_SIZE + CALC_PADDING_XZ * 2;
        private static readonly int CALC_SIZE_Z = CHUNK_SIZE + CALC_PADDING_XZ * 2;
        private static readonly int CALC_SIZE_Y = REGION_HEIGHT + CALC_PADDING_Y * 2;

        // Total counts
        private static readonly int SSBO_VOLUME = SSBO_SIZE_X * SSBO_SIZE_Z * SSBO_SIZE_Y;
        private static readonly int CALC_VOLUME = CALC_SIZE_X * CALC_SIZE_Z * CALC_SIZE_Y;

        // Internal buffers
        private readonly LightData[] _ssboData = new LightData[SSBO_VOLUME];
        private readonly LightData[] _calcData = new LightData[CALC_VOLUME];

        // Surrounding regions for voxel queries
        private readonly RegionData[] _regions = new RegionData[9];

        public void SetRegions(IEnumerable<RegionData> regions)
        {
            ClearData();

            int i = 0;
            foreach (var reg in regions)
            {
                if (i >= _regions.Length) break;
                _regions[i++] = reg;
            }
        }

        public void ClearData()
        {
            Array.Clear(_ssboData, 0, SSBO_VOLUME);
            Array.Clear(_calcData, 0, CALC_VOLUME);

            TorchQueue.Clear();
            SkyQueue.Clear();
        }

        /// <summary>
        /// Unified _indexer: accepts x/z in [-15 .. CHUNK_SIZE+14], y in [-1 .. REGION_HEIGHT].
        /// Reads always from calc buffer; writes update both calc and padded SSBO buffers.
        /// </summary>
        public LightData this[int x, int y, int z]
        {
            get => _calcData[CalcIndex(x, y, z)];
            set
            {
                // Always write to calc buffer
                _calcData[CalcIndex(x, y, z)] = value;

                // Write to SSBO if within its padded bounds
                if (InSsboBounds(x, y, z))
                {
                    _ssboData[SsboIndex(x, y, z)] = value;
                }
            }
        }

        /// <summary>
        /// Lookup from underlying RegionData (central region only)
        /// </summary>
        public bool GetVoxel(int x, int y, int z, out VoxelState state)
            => _regions[4].GetVoxel(x, y, z, out state);

        // Compute flat index for calc buffer
        private static int CalcIndex(int x, int y, int z)
            => x + CALC_PADDING_XZ
             + (z + CALC_PADDING_XZ) * CALC_SIZE_X
             + (y + CALC_PADDING_Y) * CALC_SIZE_X * CALC_SIZE_Z;

        // Compute flat index for SSBO buffer
        private static int SsboIndex(int x, int y, int z)
            => x + SSBO_PADDING
             + (z + SSBO_PADDING) * SSBO_SIZE_X
             + (y + SSBO_PADDING) * SSBO_SIZE_X * SSBO_SIZE_Z;

        // SSBO bounds include padding
        private static bool InSsboBounds(int x, int y, int z)
            => x >= -SSBO_PADDING && x < CHUNK_SIZE + SSBO_PADDING
            && z >= -SSBO_PADDING && z < CHUNK_SIZE + SSBO_PADDING
            && y >= -SSBO_PADDING && y < REGION_HEIGHT + SSBO_PADDING;

        public Span<LightData> GetSlice(int c_y)
            => _ssboData.AsSpan().Slice((c_y + REGION_NEG_CHUNKS) * PADDED_CHUNK_AREA * CHUNK_SIZE, PADDED_CHUNK_VOLUME);
    }
}
