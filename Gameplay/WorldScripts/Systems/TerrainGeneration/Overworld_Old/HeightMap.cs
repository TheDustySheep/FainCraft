using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.TerrainGeneration.Overworld_Old;
internal class HeightMap
{
    public const int BORDER = 8;
    public const int MAP_SIZE = BORDER + CHUNK_SIZE + BORDER;
    public Image<HalfSingle> Image = new(MAP_SIZE, MAP_SIZE);
    public ushort[] Heights = new ushort[MAP_SIZE * MAP_SIZE];

    public ushort GetHeight(int x, int z)
    {
        x += BORDER;
        z += BORDER;

        return Heights[z * MAP_SIZE + x];
    }
}
