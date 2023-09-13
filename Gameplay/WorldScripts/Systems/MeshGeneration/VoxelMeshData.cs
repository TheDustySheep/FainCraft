namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
public class VoxelMeshData
{
    public required VoxelVertex[] Vertices;
    public required uint[] Triangles;

    public bool IsEmpty => Vertices.Length == 0 || Triangles.Length == 0;
}
