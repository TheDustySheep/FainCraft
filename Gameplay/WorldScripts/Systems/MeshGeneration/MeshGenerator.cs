using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Voxels;
using System.Diagnostics;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
internal class MeshGenerator : IMeshGenerator
{
    readonly VoxelIndexer voxelIndexer;
    readonly VoxelData[] nVoxelDatas = new VoxelData[27];
    readonly VoxelType[] nVoxelTypes = new VoxelType[27];

    public MeshGenerator(VoxelIndexer voxelIndexer)
    {
        this.voxelIndexer = voxelIndexer;
    }

    public VoxelMeshData GenerateMesh(ChunkDataCluster cluster)
    {
#if DEBUG
        Stopwatch sw = Stopwatch.StartNew();
#endif
        List<VoxelVertex> verts = new();
        List<uint> tris = new();

        if (!cluster.CenterEmpty)
        {
            uint vertCount = 0;

            for (uint z = 0; z < CHUNK_SIZE; z++)
            {
                for (uint y = 0; y < CHUNK_SIZE; y++)
                {
                    for (uint x = 0; x < CHUNK_SIZE; x++)
                    {
                        GetNeighbourVoxels(x, y, z, cluster);

                        var voxelData = nVoxelDatas[13];
                        var voxelType = nVoxelTypes[13];

                        if (!voxelType.DrawSelf)
                            continue;

                        uint surfaceFluid = Convert.ToUInt32(voxelType.Is_Fluid && !nVoxelTypes[17].Is_Fluid);

                        for (int face = 0; face < 6; face++)
                        {
                            uint faceIndex = FACE_N_INDEX[face];
                            var faceVoxelData = nVoxelDatas[faceIndex];
                            var faceVoxelType = nVoxelTypes[faceIndex];

                            // If the face voxel is see through then draw
                            if (!faceVoxelType.SeeThrough)
                                continue;

                            if (voxelType.Skip_Draw_Similar && voxelData.Index == faceVoxelData.Index)
                                continue;

                            for (uint i = 0; i < 4; i++)
                            {
                                var vert = VERTICES[i + face * 4];
                                vert.XPos += x;
                                vert.YPos += y;
                                vert.ZPos += z;
                                vert.SurfaceFluid = surfaceFluid;

                                vert.TexIndex = voxelType.TexIDs[face];
                                verts.Add(vert);
                            }

                            for (uint i = 0; i < 6; i++)
                            {
                                tris.Add(TRIANGLES[i] + vertCount);
                            }
                            vertCount += 4;
                        }
                    }
                }
            }

        }

        var meshData = new VoxelMeshData
        {
            Vertices = verts.ToArray(),
            Triangles = tris.ToArray(),
        };

#if DEBUG
        sw.Stop();
        SystemDiagnostics.SubmitMeshGeneration(sw.Elapsed);
#endif

        return meshData;
    }

    private void GetNeighbourVoxels(uint x, uint y, uint z, ChunkDataCluster cluster)
    {
        cluster.GetVoxels(x, y, z, nVoxelDatas);

        for (int i = 0; i < 27; i++)
        {
            nVoxelTypes[i] = voxelIndexer.GetVoxelType(nVoxelDatas[i].Index);
        }
    }

    static readonly uint[] FACE_N_INDEX =
    {
        ConvertToClusterIndex(0, 1, 1),
        ConvertToClusterIndex(2, 1, 1),
        ConvertToClusterIndex(1, 0, 1),
        ConvertToClusterIndex(1, 2, 1),
        ConvertToClusterIndex(1, 1, 0),
        ConvertToClusterIndex(1, 1, 2)
    };

    static readonly VoxelVertex[] VERTICES =
    {
        // X-
        new VoxelVertex { XPos = 0, YPos = 0, ZPos = 1, Corner = 0, Normal = 1, },
        new VoxelVertex { XPos = 0, YPos = 1, ZPos = 1, Corner = 1, Normal = 1, },
        new VoxelVertex { XPos = 0, YPos = 1, ZPos = 0, Corner = 2, Normal = 1, },
        new VoxelVertex { XPos = 0, YPos = 0, ZPos = 0, Corner = 3, Normal = 1, },

        // X+
        new VoxelVertex { XPos = 1, YPos = 0, ZPos = 0, Corner = 0, Normal = 0, },
        new VoxelVertex { XPos = 1, YPos = 1, ZPos = 0, Corner = 1, Normal = 0, },
        new VoxelVertex { XPos = 1, YPos = 1, ZPos = 1, Corner = 2, Normal = 0, },
        new VoxelVertex { XPos = 1, YPos = 0, ZPos = 1, Corner = 3, Normal = 0, },

        // Y-
        new VoxelVertex { XPos = 1, YPos = 0, ZPos = 0, Corner = 0, Normal = 2, },
        new VoxelVertex { XPos = 1, YPos = 0, ZPos = 1, Corner = 1, Normal = 2, },
        new VoxelVertex { XPos = 0, YPos = 0, ZPos = 1, Corner = 2, Normal = 2, },
        new VoxelVertex { XPos = 0, YPos = 0, ZPos = 0, Corner = 3, Normal = 2, },

        // Y+
        new VoxelVertex { XPos = 0, YPos = 1, ZPos = 0, Corner = 0, Normal = 3, },
        new VoxelVertex { XPos = 0, YPos = 1, ZPos = 1, Corner = 1, Normal = 3, },
        new VoxelVertex { XPos = 1, YPos = 1, ZPos = 1, Corner = 2, Normal = 3, },
        new VoxelVertex { XPos = 1, YPos = 1, ZPos = 0, Corner = 3, Normal = 3, },

        // Z-
        new VoxelVertex { XPos = 0, YPos = 0, ZPos = 0, Corner = 0, Normal = 4, },
        new VoxelVertex { XPos = 0, YPos = 1, ZPos = 0, Corner = 1, Normal = 4, },
        new VoxelVertex { XPos = 1, YPos = 1, ZPos = 0, Corner = 2, Normal = 4, },
        new VoxelVertex { XPos = 1, YPos = 0, ZPos = 0, Corner = 3, Normal = 4, },

        // Z+
        new VoxelVertex { XPos = 1, YPos = 0, ZPos = 1, Corner = 0, Normal = 5, },
        new VoxelVertex { XPos = 1, YPos = 1, ZPos = 1, Corner = 1, Normal = 5, },
        new VoxelVertex { XPos = 0, YPos = 1, ZPos = 1, Corner = 2, Normal = 5, },
        new VoxelVertex { XPos = 0, YPos = 0, ZPos = 1, Corner = 3, Normal = 5, },
    };

    static readonly uint[] TRIANGLES =
    {
          0,  1,  2,  2,  3,  0,
    };
}
