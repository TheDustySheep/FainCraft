using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes.Types;
using FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.HeightMaps;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld.Biomes
{
    internal class BiomesDecider
    {
        readonly BiomesFactory _biomes;

        readonly NoiseMapHumidity    _noiseMapHumidity;
        readonly NoiseMapMountains   _noiseMapMountains; 
        readonly NoiseMapContinental _noiseMapContinental;
        readonly NoiseMapTemperature _noiseMapTemperature;

        public BiomesDecider(BiomesFactory biomeFactory, int seed)
        {
            _biomes = biomeFactory;

            _noiseMapHumidity    = new NoiseMapHumidity(seed);
            _noiseMapMountains   = new NoiseMapMountains(seed);
            _noiseMapContinental = new NoiseMapContinental(seed);
            _noiseMapTemperature = new NoiseMapTemperature(seed);
        }

        public void SampleBiomes(RegionMaps maps, RegionCoord regionCoord)
        {
            VoxelCoordGlobal globalCoord = (VoxelCoordGlobal)regionCoord;

            SampleMaps(maps, regionCoord);

            for (int x = -RegionMaps.OVERSAMPLE_RADIUS; x < CHUNK_SIZE + RegionMaps.OVERSAMPLE_RADIUS; x++)
            {
                for (int z = -RegionMaps.OVERSAMPLE_RADIUS; z < CHUNK_SIZE + RegionMaps.OVERSAMPLE_RADIUS; z++)
                {
                    var coord = new VoxelCoord2DGlobal(
                        globalCoord.X + x,
                        globalCoord.Z + z
                    );

                    maps.SetBiomeOversampled(
                        x + RegionMaps.OVERSAMPLE_RADIUS,
                        z + RegionMaps.OVERSAMPLE_RADIUS, 
                        GetBiome(coord));
                }
            }
        }

        private void SampleMaps(RegionMaps maps, RegionCoord regionCoord)
        {
            for (int l_x = 0; l_x < CHUNK_SIZE; l_x++)
            {
                for (int l_z = 0; l_z < CHUNK_SIZE; l_z++)
                {
                    VoxelCoord2DGlobal coord = new(regionCoord, l_x, l_z);

                    maps.Humidity   [l_x, l_z] = _noiseMapHumidity   .Sample(coord);
                    maps.Mountains  [l_x, l_z] = _noiseMapMountains  .Sample(coord);
                    maps.Continents [l_x, l_z] = _noiseMapContinental.Sample(coord);
                    maps.Temperature[l_x, l_z] = _noiseMapTemperature.Sample(coord);
                }
            }
        }

        public IBiome GetBiome(VoxelCoord2DGlobal globalCoord2D)
        {
            float humidity    = _noiseMapHumidity   .Sample(globalCoord2D);
            float mountains   = _noiseMapMountains  .Sample(globalCoord2D);
            float continent   = _noiseMapContinental.Sample(globalCoord2D);
            float temperature = _noiseMapTemperature.Sample(globalCoord2D);

            // Ocean
            if (continent < -0.4)
                return _biomes.Ocean;
            // Coastal
            else if (continent < -0.38)
            {
                if (mountains > 0)
                {
                    return _biomes.Hills;
                }
                else
                {
                    return _biomes.SandyShores;
                }
            }
            // Deep Inland
            else
            {
                if (mountains > 0)
                {
                    return _biomes.Hills;
                }
                else
                {
                    if (temperature > 0)
                        return _biomes.Desert;
                    else
                        return _biomes.Plains;
                }
            }
        }
    }
}
