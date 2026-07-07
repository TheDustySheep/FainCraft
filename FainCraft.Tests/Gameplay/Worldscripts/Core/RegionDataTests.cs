using FainCraft.Gameplay.WorldScripts.Data.Chunks;
using FainCraft.Gameplay.WorldScripts.Data.Regions;
using FainCraft.Gameplay.WorldScripts.Data.Voxels;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraftTests.Gameplay.Worldscripts.Core
{
    public class RegionDataTests
    {
        [Fact]
        public void GetChunk_ReturnsTrue_ForValidIndices()
        {
            var regionData = new RegionData();

            for (int c_y = -REGION_NEG_CHUNKS; c_y < REGION_POS_CHUNKS; c_y++)
            {
                bool result = regionData.GetChunk(c_y, out var chunk);
                Assert.True(result, $"Expected GetChunk({c_y}) to succeed.");
                Assert.NotNull(chunk);
            }
        }

        [Fact]
        public void GetChunk_ReturnsFalse_ForInvalidIndices()
        {
            var regionData = new RegionData();

            Assert.False(regionData.GetChunk(-REGION_NEG_CHUNKS - 1, out _));
            Assert.False(regionData.GetChunk(REGION_POS_CHUNKS, out _));
        }

        [Fact]
        public void Voxel_WriteAndRead_ReturnsSameData()
        {
            var regionData = new RegionData();

            for (int i_y = 0; i_y < REGION_VOXEL_HEIGHT; i_y++)
            {
                int c_y = (i_y >> 5) - REGION_NEG_CHUNKS;
                Assert.True(regionData.GetChunk(c_y, out var chunk), $"GetChunk({c_y}) failed");

                int l_y = i_y % 32;
                var expectedVoxel = new VoxelState() { Index = (uint)i_y };

                chunk[0, l_y, 0] = expectedVoxel;

                Assert.True(regionData.GetVoxel(0, i_y, 0, out var actualVoxel), $"GetVoxelState at y={i_y} failed.");
                Assert.Equal(expectedVoxel.Index, actualVoxel.Index);
            }
        }

        [Fact]
        public void GetVoxel_ReturnsFalse_ForInvalidIndices()
        {
            var regionData = new RegionData();

            Assert.False(regionData.GetVoxel(0, -1, 0, out _));
            Assert.False(regionData.GetVoxel(0, REGION_VOXEL_HEIGHT, 0, out _));
        }

        [Fact]
        public void SetChunk_OverwritesChunkCorrectly()
        {
            var regionData = new RegionData();
            var customChunk = new ChunkData();
            customChunk[0, 0, 0] = new VoxelState() { Index = 123 };

            bool setResult = regionData.SetChunk(0, customChunk);
            Assert.True(setResult, "SetChunk failed");

            bool getResult = regionData.GetChunk(0, out var retrievedChunk);
            Assert.True(getResult, "GetChunk failed after SetChunk");

            Assert.Equal(123u, retrievedChunk[0, 0, 0].Index);
        }

        [Fact]
        public void Constructor_Throws_IfInvalidArrayLength()
        {
            var invalidArray = new ChunkData[REGION_TOTAL_CHUNKS - 1];

            var ex = Assert.Throws<Exception>(() => new RegionData(invalidArray));
            Assert.Contains("Expected", ex.Message);
        }
    }
}
