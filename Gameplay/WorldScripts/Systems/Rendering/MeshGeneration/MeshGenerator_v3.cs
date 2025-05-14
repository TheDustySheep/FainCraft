using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Voxels;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.MeshGeneration;
public class MeshGenerator_v3 : IMeshGenerator
{
    readonly IVoxelIndexer _voxelIndexer;
    readonly VoxelState[] nVoxelDatas = new VoxelState[27];
    readonly  VoxelType[] nVoxelTypes = new  VoxelType[27];

    readonly VoxelState[] allVoxelDatas = new VoxelState[(CHUNK_SIZE + 2) * (CHUNK_SIZE + 2) * (CHUNK_SIZE + 2)];
    readonly  VoxelType[] allVoxelTypes = new  VoxelType[(CHUNK_SIZE + 2) * (CHUNK_SIZE + 2) * (CHUNK_SIZE + 2)];

    const uint VOXEL_UP = 22;

    public MeshGenerator_v3(IVoxelIndexer voxelIndexer)
    {
        _voxelIndexer = voxelIndexer;
    }

    public void GenerateMesh(IChunkDataCluster cluster, VoxelMeshData opaqueMeshData, VoxelMeshData transparentMeshData)
    {
        opaqueMeshData.Clear();
        transparentMeshData.Clear();

        var o_tris  = opaqueMeshData.Triangles;
        var o_verts = opaqueMeshData.Vertices;

        var t_tris  = transparentMeshData.Triangles;
        var t_verts = transparentMeshData.Vertices;

        SetVoxels(cluster);

        uint o_vertCount = 0;
        uint t_vertCount = 0;

        bool[] customMeshLookup = _voxelIndexer.CustomMesh.Data;

        for (int y = 0; y < CHUNK_SIZE; y++)
        {
            for (int z = 0; z < CHUNK_SIZE; z++)
            {
                for (int x = 0; x < CHUNK_SIZE; x++)
                {
                    var voxelData = cluster.GetVoxel(x, y, z);

                    if (customMeshLookup[voxelData.Index])
                        throw new NotImplementedException("Custom Voxel Meshes are not yet implemented");

                    var voxelType = _voxelIndexer.GetVoxelType(voxelData.Index);

                    if (!voxelType.DrawSelf)
                        continue;

                    var verts = voxelType.Is_Transparent ? t_verts : o_verts;

                    GetNeighbourVoxels(x, y, z);

                    uint surfaceFluid = Convert.ToUInt32(voxelType.Is_Fluid && !nVoxelTypes[VOXEL_UP].Is_Fluid);

                    for (uint face = 0; face < 6; face++)
                    {
                        uint faceIndex    = FACE_N_INDEX[face];
                        var faceVoxelData = nVoxelDatas[faceIndex];
                        var faceVoxelType = nVoxelTypes[faceIndex];

                        // If the face voxel is see through then draw
                        if (faceVoxelType.Fully_Opaque)
                            continue;

                        if (voxelType.Skip_Draw_Similar && voxelData.Index == faceVoxelData.Index)
                            continue;

                        uint animate_foliage = Convert.ToUInt32(voxelType.Animate_Foliage);
                        uint blend_biome     = Convert.ToUInt32(voxelType.Biome_Blend[face]);

                        for (uint i = 0; i < 4; i++)
                        {
                            var vert = new VoxelVertex()
                            {
                                Corner = i,
                                MeshIndex = face,
                                XPos = (uint)x,
                                YPos = (uint)y,
                                ZPos = (uint)z,
                                SurfaceFluid = surfaceFluid,
                                TexIndex = voxelType.TexIDs[face]
                            };

                            uint side1 = Convert.ToUInt32(nVoxelTypes[AO_LOOKUP[face * 12 + i * 3 + 0]].Fully_Opaque);
                            uint cornr = Convert.ToUInt32(nVoxelTypes[AO_LOOKUP[face * 12 + i * 3 + 1]].Fully_Opaque);
                            uint side2 = Convert.ToUInt32(nVoxelTypes[AO_LOOKUP[face * 12 + i * 3 + 2]].Fully_Opaque);
                            vert.AO = GetBranchlessVertexAO(side1, side2, cornr);

                            vert.Biome_Blend = blend_biome;
                            vert.Animate_Foliage = animate_foliage;

                            verts.Add(vert);
                        }

                        if (voxelType.Is_Transparent)
                        {
                            t_tris.Add(TRIANGLES[0] + t_vertCount);
                            t_tris.Add(TRIANGLES[1] + t_vertCount);
                            t_tris.Add(TRIANGLES[2] + t_vertCount);
                            t_tris.Add(TRIANGLES[3] + t_vertCount);
                            t_tris.Add(TRIANGLES[4] + t_vertCount);
                            t_tris.Add(TRIANGLES[5] + t_vertCount);

                            t_vertCount += 4;
                        }
                        else
                        {
                            o_tris.Add(TRIANGLES[0] + o_vertCount);
                            o_tris.Add(TRIANGLES[1] + o_vertCount);
                            o_tris.Add(TRIANGLES[2] + o_vertCount);
                            o_tris.Add(TRIANGLES[3] + o_vertCount);
                            o_tris.Add(TRIANGLES[4] + o_vertCount);
                            o_tris.Add(TRIANGLES[5] + o_vertCount);

                            o_vertCount += 4;
                        }
                    }
                }
            }
        }
    }

    private void SetVoxels(IChunkDataCluster cluster)
    {
        int i = 0;
        Span<VoxelState> voxelDatas = allVoxelDatas;
        Span<VoxelType> voxelTypes = allVoxelTypes;

        for (int y = -1; y < CHUNK_SIZE + 1; y++)
        {
            for (int z = -1; z < CHUNK_SIZE + 1; z++)
            {
                for (int x = -1; x < CHUNK_SIZE + 1; x++, i++)
                {
                    var voxelData = cluster.GetVoxel(x, y, z);
                    voxelDatas[i] = voxelData;
                    voxelTypes[i] = _voxelIndexer.GetVoxelType(voxelData.Index);
                }
            }
        }
    }

    private static uint GetBranchlessVertexAO(uint side1, uint side2, uint corner)
    {
        if (side1 + side2 > 0)
            return 0;

        return 3 - (side1 + side2 + corner);
    }

    private void GetNeighbourVoxels(int x, int y, int z)
    {
        int index = 0;
        for (int y_off = 0; y_off < 3; y_off++)
        {
            int y_local = (y + y_off) * (CHUNK_SIZE + 2) * (CHUNK_SIZE + 2);

            for (int z_off = 0; z_off < 3; z_off++)
            {
                int z_local = (z + z_off) * (CHUNK_SIZE + 2);

                int baseIndex = x + z_local + y_local;

                nVoxelDatas[index + 0] = allVoxelDatas[baseIndex + 0];
                nVoxelTypes[index + 0] = allVoxelTypes[baseIndex + 0];

                nVoxelDatas[index + 1] = allVoxelDatas[baseIndex + 1];
                nVoxelTypes[index + 1] = allVoxelTypes[baseIndex + 1];

                nVoxelDatas[index + 2] = allVoxelDatas[baseIndex + 2];
                nVoxelTypes[index + 2] = allVoxelTypes[baseIndex + 2];

                index += 3;
            }
        }
    }

    static readonly uint[] FACE_N_INDEX =
    {
        // X
        ClusterIndex(0, 1, 1),
        ClusterIndex(2, 1, 1),

        // Y
        ClusterIndex(1, 0, 1),
        ClusterIndex(1, 2, 1),

        // Z
        ClusterIndex(1, 1, 0),
        ClusterIndex(1, 1, 2)
    };

    static readonly uint[] AO_LOOKUP =
    {
        // X-
        ClusterIndex(0, 0, 1), ClusterIndex(0, 0, 2), ClusterIndex(0, 1, 2),
        ClusterIndex(0, 1, 2), ClusterIndex(0, 2, 2), ClusterIndex(0, 2, 1),
        ClusterIndex(0, 2, 1), ClusterIndex(0, 2, 0), ClusterIndex(0, 1, 0),
        ClusterIndex(0, 1, 0), ClusterIndex(0, 0, 0), ClusterIndex(0, 0, 1),
        
        // X+
        ClusterIndex(2, 0, 1), ClusterIndex(2, 0, 0), ClusterIndex(2, 1, 0),
        ClusterIndex(2, 1, 0), ClusterIndex(2, 2, 0), ClusterIndex(2, 2, 1),
        ClusterIndex(2, 2, 1), ClusterIndex(2, 2, 2), ClusterIndex(2, 1, 2),
        ClusterIndex(2, 1, 2), ClusterIndex(2, 0, 2), ClusterIndex(2, 0, 1),

        // Y- 
        ClusterIndex(1, 0, 0), ClusterIndex(2, 0, 0), ClusterIndex(2, 0, 1),
        ClusterIndex(2, 0, 1), ClusterIndex(2, 0, 2), ClusterIndex(1, 0, 2),
        ClusterIndex(1, 0, 2), ClusterIndex(0, 0, 2), ClusterIndex(0, 0, 1),
        ClusterIndex(0, 0, 1), ClusterIndex(0, 0, 0), ClusterIndex(1, 0, 0),

        // Y+
        ClusterIndex(1, 2, 0), ClusterIndex(0, 2, 0), ClusterIndex(0, 2, 1),
        ClusterIndex(0, 2, 1), ClusterIndex(0, 2, 2), ClusterIndex(1, 2, 2),
        ClusterIndex(1, 2, 2), ClusterIndex(2, 2, 2), ClusterIndex(2, 2, 1),
        ClusterIndex(2, 2, 1), ClusterIndex(2, 2, 0), ClusterIndex(1, 2, 0),

        // Z- 
        ClusterIndex(1, 0, 0), ClusterIndex(0, 0, 0), ClusterIndex(0, 1, 0),
        ClusterIndex(0, 1, 0), ClusterIndex(0, 2, 0), ClusterIndex(1, 2, 0),
        ClusterIndex(1, 2, 0), ClusterIndex(2, 2, 0), ClusterIndex(2, 1, 0),
        ClusterIndex(2, 1, 0), ClusterIndex(2, 0, 0), ClusterIndex(1, 0, 0),

        // Z+
        ClusterIndex(1, 0, 2), ClusterIndex(2, 0, 2), ClusterIndex(2, 1, 2),
        ClusterIndex(2, 1, 2), ClusterIndex(2, 2, 2), ClusterIndex(1, 2, 2),
        ClusterIndex(1, 2, 2), ClusterIndex(0, 2, 2), ClusterIndex(0, 1, 2),
        ClusterIndex(0, 1, 2), ClusterIndex(0, 0, 2), ClusterIndex(1, 0, 2),
    };

    static readonly uint[] TRIANGLES =
    {
          0,  1,  2,  2,  3,  0,
    };
}
