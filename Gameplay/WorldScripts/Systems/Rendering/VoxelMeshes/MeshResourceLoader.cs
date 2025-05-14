using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes
{
    public static class MeshResourceLoader
    {
        private struct Quad
        {
            public Vector3 A, B, C, D;
            public Vector2 UV_Min, UV_Max;
            public Vector3 Normal;

            public Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 uvMin, Vector2 uvMax)
            {
                A = a;
                B = b;
                C = c;
                D = d;
                UV_Min = uvMin;
                UV_Max = uvMax;
                Normal = Vector3.Normalize(Vector3.Cross(B - A, C - A));
            }
        }

        public static List<MeshQuad> LoadMeshQuads(string fileName)
        {
            var quads = LoadFromFile(fileName);
            return quads.ConvertAll(i => new MeshQuad(i.A, i.B, i.C, i.D, i.UV_Min, i.UV_Max));
        }

        private static List<Quad> LoadFromFile(string fileName)
        {
            string relativePath = Path.Combine("Resources", "Voxels", "Meshes", fileName);
            if (!File.Exists(relativePath))
                throw new FileNotFoundException($"OBJ file not found at '{relativePath}'");

            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();
            var quads = new List<Quad>();

            foreach (var line in File.ReadLines(relativePath))
            {
                if (line.StartsWith("v "))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    verts.Add(new Vector3(x, y, z));
                }
                else if (line.StartsWith("vt "))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    float u = float.Parse(parts[1]);
                    float v = float.Parse(parts[2]);
                    uvs.Add(new Vector2(u, v));
                }
                else if (line.StartsWith("f "))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 5)
                        continue; // Skip non-quad faces

                    var faceVerts = new Vector3[4];
                    var faceUVs = new Vector2[4];

                    for (int i = 0; i < 4; i++)
                    {
                        var tokens = parts[i + 1].Split('/');
                        int vIndex = int.Parse(tokens[0]) - 1;
                        int uvIndex = int.Parse(tokens[1]) - 1;

                        faceVerts[i] = verts[vIndex];
                        faceUVs[i] = uvs[uvIndex];
                    }

                    var quad = new Quad(
                        faceVerts[0],
                        faceVerts[1],
                        faceVerts[2],
                        faceVerts[3],
                        faceUVs[0], // UV_Min (from vertex 1)
                        faceUVs[2]  // UV_Max (from vertex 3)
                    );

                    quads.Add(quad);
                }
            }

            return quads;
        }
    }
}
