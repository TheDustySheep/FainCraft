using FainCraft.Gameplay.WorldScripts.Core;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Editing;
internal struct VoxelEditResult
{
    public bool Immediate;
    public bool[] UpdateChunks;

    public VoxelEditResult(LocalVoxelCoord coord, bool immediate = false)
    {
        Immediate = immediate;
        UpdateChunks = new bool[6]
        {
            coord.X == 0,
            coord.Y == 0,
            coord.Z == 0,
            coord.X == CHUNK_SIZE - 1,
            coord.Y == CHUNK_SIZE - 1,
            coord.Z == CHUNK_SIZE - 1
        };
    }
}