using FainCraft.Gameplay.WorldScripts.Data;
using FainCraft.Gameplay.WorldScripts.Voxels;
using System.Diagnostics;
using static FainCraft.Gameplay.WorldScripts.Core.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Systems.MeshGeneration;
public class MeshGenerator_Bitwise : IMeshGenerator
{
    readonly VoxelIndexer voxelIndexer;

    public MeshGenerator_Bitwise(VoxelIndexer voxelIndexer)
    {
        this.voxelIndexer = voxelIndexer;
    }

    public VoxelMeshData GenerateMesh(ChunkDataCluster cluster)
    {
        Stopwatch sw = new();

        var tris = new List<uint>();
        var verts = new List<VoxelVertex>();

        if (cluster.CenterEmpty)
            return new VoxelMeshData();

        // Bit Chunk
        sw.Start();

        BitChunk isSolid = new((int x, int y, int z) =>
        {
            var voxelData = cluster.GetVoxel(x - 1, y - 1, z - 1);
            var voxelType = voxelIndexer.GetVoxelType(voxelData.Index);

            return voxelType.DrawSelf;
        });

        sw.Stop();

        var bitChunkTime = sw.Elapsed;

        // Mesh Generation
        sw.Restart();

        // X Faces
        uint vertCount = 0;
        for (uint y = 1; y < CHUNK_SIZE_PAD - 1; y++)
        {
            for (uint z = 1; z < CHUNK_SIZE_PAD - 1; z++)
            {
                ulong drawBits = CalculateRightFaces(isSolid.xBitsArray[y + z * CHUNK_SIZE_PAD]);

                for (uint x = 1; x < CHUNK_SIZE_PAD - 1; x++)
                {
                    bool draw = (drawBits & (1u << (int)x)) > 0;

                    if (!draw)
                        continue;

                    for (uint i = 0; i < 4; i++)
                    {
                        var vert = VERTICES[i + 0 * 4];
                        vert.XPos += x - 1;
                        vert.YPos += y - 1;
                        vert.ZPos += z - 1;
                        //vert.SurfaceFluid = surfaceFluid;
                        vert.TexIndex = 54;
                        //vert.TexIndex = voxelType.TexIDs[face];

                        //uint side1 = Convert.ToUInt32(nVoxelTypes[AO_LOOKUP[face * 12 + i * 3 + 0]].Fully_Opaque);
                        //uint cornr = Convert.ToUInt32(nVoxelTypes[AO_LOOKUP[face * 12 + i * 3 + 1]].Fully_Opaque);
                        //uint side2 = Convert.ToUInt32(nVoxelTypes[AO_LOOKUP[face * 12 + i * 3 + 2]].Fully_Opaque);
                        //vert.AO = GetVertexAO(side1, side2, cornr);

                        //vert.Biome_Blend = blend_biome;
                        //vert.Animate_Foliage = animate_foliage;

                        verts.Add(vert);
                    }

                    tris.Add(TRIANGLES[0] + vertCount);
                    tris.Add(TRIANGLES[1] + vertCount);
                    tris.Add(TRIANGLES[2] + vertCount);
                    tris.Add(TRIANGLES[3] + vertCount);
                    tris.Add(TRIANGLES[4] + vertCount);
                    tris.Add(TRIANGLES[5] + vertCount);
                    vertCount += 4;
                }
            }
        }

        sw.Stop();

        var meshTime = sw.Elapsed;

        SystemDiagnostics.SubmitMeshGeneration(new MeshGenDebugData
        {
            TotalTime = bitChunkTime + meshTime,
        });

        return new VoxelMeshData(verts.ToArray(), tris.ToArray());
    }

    public ulong CalculateLeftFaces(ulong isSolid)
    {
        return ~(isSolid >> 1) & isSolid;
    }
    public ulong CalculateRightFaces(ulong isSolid)
    {
        return ~(isSolid << 1) & isSolid;
    }

    public class BitChunk
    {
        public ulong[] xBitsArray = new ulong[CHUNK_SIZE_PAD * CHUNK_SIZE_PAD];
        public ulong[] yBitsArray = new ulong[CHUNK_SIZE_PAD * CHUNK_SIZE_PAD];
        public ulong[] zBitsArray = new ulong[CHUNK_SIZE_PAD * CHUNK_SIZE_PAD];

        public BitChunk(ChunkDataCluster cluster, VoxelIndexer indexer)
        {
            for (int x = 0; x < CHUNK_SIZE_PAD; x++)
            {
                for (int y = 0; y < CHUNK_SIZE_PAD; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE_PAD; z++)
                    {
                        var voxelData = cluster.GetVoxel(x - 1, y - 1, z - 1);
                        var voxelType = indexer.GetVoxelType(voxelData.Index);

                        ulong drawSelf = voxelType.DrawSelf ? 1u : 0;

                        xBitsArray[y + z * CHUNK_SIZE_PAD] |= drawSelf << x;
                        yBitsArray[x + z * CHUNK_SIZE_PAD] |= drawSelf << y;
                        zBitsArray[x + y * CHUNK_SIZE_PAD] |= drawSelf << z;
                    }
                }
            }
        }

        public BitChunk(Func<int, int, int, bool> predicate)
        {
            for (int x = 0; x < CHUNK_SIZE_PAD; x++)
            {
                for (int y = 0; y < CHUNK_SIZE_PAD; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE_PAD; z++)
                    {
                        ulong bit = predicate.Invoke(x, y, z) ? 1u : 0;

                        xBitsArray[y + z * CHUNK_SIZE_PAD] |= bit << x;
                        yBitsArray[x + z * CHUNK_SIZE_PAD] |= bit << y;
                        zBitsArray[x + y * CHUNK_SIZE_PAD] |= bit << z;
                    }
                }
            }
        }

        public void PrintSlice(int z)
        {
            for (int x = 0; x < CHUNK_SIZE_PAD; x++)
            {
                var str = $"X: {x} - ";
                for (int y = 0; y < CHUNK_SIZE_PAD; y++)
                {
                    str += (zBitsArray[x + y * CHUNK_SIZE_PAD] & (1u << z)) > 0 ? 1 : 0;
                }
                Console.WriteLine(str);
            }
        }
    }

    private static uint GetVertexAO(uint side1, uint side2, uint corner)
    {
        if ((side1 & side2) == 1)
            return 0;

        return 3 - (side1 + side2 + corner);
    }


    static readonly uint[] FACE_N_INDEX =
    {
        ClusterIndex(0, 1, 1),
        ClusterIndex(2, 1, 1),
        ClusterIndex(1, 0, 1),
        ClusterIndex(1, 2, 1),
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
