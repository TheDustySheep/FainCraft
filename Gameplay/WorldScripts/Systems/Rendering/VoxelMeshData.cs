namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering;
public class VoxelMeshData
{
    public readonly List<VoxelVertex> Vertices  = new();
    public readonly List<uint>        Triangles = new();

    public void Clear()
    {
        Vertices .Clear(); 
        Triangles.Clear();
    }

    public bool IsEmpty => 
        Vertices.Count == 0 || 
        Triangles.Count == 0;
}
