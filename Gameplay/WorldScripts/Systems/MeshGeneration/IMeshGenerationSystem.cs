using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
internal interface IMeshGenerationSystem
{
    public void Generate(ChunkCoord coord, bool immediate = false);
    public void Tick();
}
