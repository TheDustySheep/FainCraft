using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Data.Chunks;

namespace FainCraft.Gameplay.WorldScripts.Signals;

public struct ModifiedChunkDataSignal
{
    public ChunkCoord Coord;
    public ChunkData Data;
}
