using FainCraft.Gameplay.WorldScripts.Chunking;
using FainCraft.Gameplay.WorldScripts.Voxels;
using Silk.NET.Assimp;
using System;
using System.Diagnostics;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
internal class MeshGenerator : IMeshGenerator
{
    VoxelIndexer voxelIndexer;
    VoxelData[] nVoxelDatas = new VoxelData[27];
    VoxelType[] nVoxelTypes = new VoxelType[27];

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
                        GetNeighbourVoxels((int)x, (int)y, (int)z, cluster);

                        var voxelData = nVoxelDatas[13];
                        var voxelType = nVoxelTypes[13];

                        if (!voxelType.DrawSelf)
                            continue;

                        for (int face = 0; face < 6; face++)
                        {
                            uint faceIndex = FACE_N_INDEX[face];
                            var faceVoxelData = nVoxelDatas[faceIndex];
                            var faceVoxelType = nVoxelTypes[faceIndex];

                            if (faceVoxelType.DrawSolid)
                                continue;

                            for (uint i = 0; i < 4; i++)
                            {
                                var vert = VERTICES[i + face * 4];
                                vert.XPos += x;
                                vert.YPos += y;
                                vert.ZPos += z;

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

    private void GetNeighbourVoxels(int x, int y, int z, ChunkDataCluster cluster)
    {
        cluster.GetVoxels(x, y, z, nVoxelDatas);

        for (int i = 0; i < 27; i++)
        {
            var data = nVoxelDatas[i];
            var type = voxelIndexer.GetVoxelType(data.Index);
            nVoxelTypes[i] = type;
        }
    }

    static readonly uint[] FACE_N_INDEX =
    {
        0 * 1 + 1 * 3 + 1 * 9,
        2 * 1 + 1 * 3 + 1 * 9,
        1 * 1 + 0 * 3 + 1 * 9,
        1 * 1 + 2 * 3 + 1 * 9,
        1 * 1 + 1 * 3 + 0 * 9,
        1 * 1 + 1 * 3 + 2 * 9,
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

    //static readonly uint[] TRIANGLES =
    //{
    //      0,  1,  2,  2,  3,  0,
    //      4,  5,  6,  6,  7,  4,
    //      8,  9, 10, 10, 11,  8,
    //     12, 13, 14, 14, 15, 12,
    //     16, 17, 18, 18, 19, 16,
    //     20, 21, 22, 22, 23, 20,
    //};
}
