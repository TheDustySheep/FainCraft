using FainCraft.Gameplay.WorldScripts.Coords;
using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGenerationSystems;
public interface IMeshGenerationSystem
{
    public void Tick();

    public void Update(ChunkCoord coord);
    public void Update(RegionCoord coord);

    public void Cancel(RegionCoord coord);

    public event Action<ChunkCoord, VoxelMeshData, VoxelMeshData>? OnComplete;
}
