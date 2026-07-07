using FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting;
using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraftTests.Gameplay.Worldscripts.Lighting;

public class RegionLightingDataTests
{
    [Fact]
    public void Indexer_WriteAndRead_ReturnsSameValue()
    {
        var regionLightingData = new LightingRegionData();
        var lightValue = new LightData() { Sky = 42 };

        int testX = 1;
        int testY = 10;
        int testZ = 2;

        regionLightingData[testX, testY, testZ] = lightValue;

        var retrievedValue = regionLightingData[testX, testY, testZ];

        Assert.Equal(lightValue.Sky, retrievedValue.Sky);
    }

    [Fact]
    public void GetSlice_ReturnsCorrectSlice()
    {
        var regionLightingData = new LightingRegionData();
        int c_y = 0; // middle chunk

        var slice = regionLightingData.GetSlice(c_y);

        Assert.Equal(PADDED_CHUNK_VOLUME, slice.Length);

        // Fill slice with known values
        for (int i = 0; i < slice.Length; i++)
        {
            slice[i] = new LightData() { Sky = (byte)(i % 256) };
        }

        // Verify values accessible via indexer
        int baseY = (c_y + REGION_NEG_CHUNKS) * PADDED_CHUNK_SIZE;

        for (int l_y = 0; l_y < PADDED_CHUNK_SIZE; l_y++)
        {
            for (int l_z = 0; l_z < PADDED_CHUNK_SIZE; l_z++)
            {
                for (int l_x = 0; l_x < PADDED_CHUNK_SIZE; l_x++)
                {
                    var expected = slice[l_x + l_z * PADDED_CHUNK_SIZE + l_y * PADDED_CHUNK_AREA];
                    var actual = regionLightingData[l_x, baseY + l_y, l_z];

                    Assert.Equal(expected.Sky, actual.Sky);
                }
            }
        }
    }

    [Fact]
    public void GetSlice_ThrowsIfOutOfRange()
    {
        var regionLightingData = new LightingRegionData();

        // Assuming negative or too-high c_y would throw in actual use
        // But since Slice() will throw ArgumentOutOfRangeException:
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            regionLightingData.GetSlice(REGION_POS_CHUNKS);
        });

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            regionLightingData.GetSlice(-REGION_NEG_CHUNKS - 1);
        });
    }
}
