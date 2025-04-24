namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
public class VoxelMeshData
{
    public VoxelVertex[] Vertices;
    public uint[] Triangles;

    public VoxelMeshData()
    {
        Vertices = Array.Empty<VoxelVertex>();
        Triangles = Array.Empty<uint>();
    }

    public VoxelMeshData(VoxelVertex[] vertices, uint[] triangles)
    {
        Vertices = vertices;
        Triangles = triangles;
    }

    public bool IsEmpty => Vertices.Length == 0 || Triangles.Length == 0;
}
