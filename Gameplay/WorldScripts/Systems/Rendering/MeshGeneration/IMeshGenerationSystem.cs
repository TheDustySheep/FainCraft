using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
internal interface IMeshGenerationSystem
{
    public void Generate(ChunkCoord coord, bool important);
    public void Tick();
}
