using FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes;
using Newtonsoft.Json;

namespace FainCraft.Gameplay.WorldScripts.Voxels
{
    public static class VoxelTypeLoader
    {
        public static IVoxelIndexer LoadIndexer(string filePath = @"Resources\Voxels\Voxels.json")
        {
            LoadFile(filePath, out VoxelType[] voxelTypes, out MeshQuad[] quadIndex);
            return new VoxelIndexer(voxelTypes, quadIndex);
        }

        private static void LoadFile(string filePath, out VoxelType[] voxelTypes, out MeshQuad[] quadIndex)
        {
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error
            };

            string voxel_text = File.ReadAllText(filePath);
            var voxels = JsonConvert.DeserializeObject<VoxelLoadType[]>(voxel_text, settings);

            if (voxels == null)
            {
                Console.WriteLine("Could not load voxels");
                quadIndex  = [];
                voxelTypes = [];
                return;
            }

            GenerateQuadIndex(voxels, out var mapping, out var _quadIndex);

            voxelTypes = Array.ConvertAll(voxels, i => Convert(i, mapping, _quadIndex));
            quadIndex  = _quadIndex;
        }

        private static void GenerateQuadIndex(
            VoxelLoadType[] voxels, 
            out Dictionary<VoxelLoadType, List<uint>> mapping,
            out MeshQuad[] index)
        {
            const int OFFSET_VOXELS = 6;

            var meshes = voxels
                .Where(i => i.Custom_Mesh != null)
                .ToDictionary(i => i, i => MeshResourceLoader.LoadMeshQuads(i.Custom_Mesh!))
                .Where(i => i.Value.Count > 0);

            // 1) Prepare containers
            var uniqueMeshQuads = new List<MeshQuad>();
            var quadToIndex = new Dictionary<MeshQuad, uint>();  // uses your MeshQuad ==

            mapping = new Dictionary<VoxelLoadType, List<uint>>();

            // 2) Deduplicate and build mapping
            foreach (var kvp in meshes)
            {
                var voxel = kvp.Key;
                var quads = kvp.Value;
                var indices = new List<uint>(quads.Count);

                foreach (var quad in quads)
                {
                    if (!quadToIndex.TryGetValue(quad, out uint idx))
                    {
                        idx = (uint)uniqueMeshQuads.Count;
                        uniqueMeshQuads.Add(quad);
                        quadToIndex[quad] = idx;
                    }
                    indices.Add(idx);
                }

                mapping[voxel] = indices;
            }

            index = uniqueMeshQuads.ToArray();

            foreach (var kv in mapping)
            {
                for (int i = 0; i < kv.Value.Count; i++)
                    kv.Value[i] += OFFSET_VOXELS;
            }
        }

        private static VoxelType Convert(VoxelLoadType load, Dictionary<VoxelLoadType, List<uint>> mappings, MeshQuad[] index)
        {
            uint[] texIds = load.TexIDs;
            if (load.Custom_Mesh == null && texIds.Length == 1)
            {
                // All faces the same if array is 1 in length
                uint id = texIds[0];
                texIds = [id, id, id, id, id, id];
            }

            bool[] biome_blend = load.Foliage_Biome_Blend;
            if (biome_blend.Length == 1)
            {
                // All faces the same if array is 1 in length
                bool blend = biome_blend[0];
                biome_blend = [blend, blend, blend, blend, blend, blend];
            }

            CustomVoxel? voxel = null;
            if (mappings.TryGetValue(load, out var quads))
            {
                voxel = new CustomVoxel(quads.ToArray());

                if (quads.Count != texIds.Length)
                    throw new Exception("Each quad is required to have a corrosponding texture");
            }


            return new VoxelType()
            {
                Name             = load.Name,

                Physics_Solid    = load.Physics_Solid,

                TexIDs           = texIds,

                Draw_Self        = load.Draw_Self,
                Draw_Similar     = load.Draw_Similar,
                Draw_Opaque      = load.Draw_Opaque,
                Draw_Transparent = load.Draw_Transparent,

                Foliage_Animate     = load.Foliage_Animate,
                Foliage_Biome_Blend = biome_blend,

                Is_Fluid       = load.Is_Fluid,

                Light_Emission = load.Light_Emits,
                Light_Solid    = !load.Light_Opaque,

                Custom_Mesh    = voxel,
            };
        }

        private class VoxelLoadType
        {
            // Name
            [JsonProperty("Name")]                public required string Name;

            // Drawing
            [JsonProperty("Draw_Self")]           public bool Draw_Self = true;
            [JsonProperty("Draw_Similar")]        public bool Draw_Similar = true;
            [JsonProperty("Draw_Opaque")]         public bool Draw_Opaque = true;
            [JsonProperty("Draw_Transparent")]    public bool Draw_Transparent;

            // Foliage
            [JsonProperty("Foliage_Animate")]     public bool   Foliage_Animate;
            [JsonProperty("Foliage_Biome_Blend")] public bool[] Foliage_Biome_Blend = [false];

            // Physics
            [JsonProperty("Physics_Solid")]       public bool Physics_Solid = true;
                                                  
            // Fluid                              
            [JsonProperty("Is_Fluid")]            public bool Is_Fluid;
                                                  
            // Texturing                          
            [JsonProperty("Textures")]            public uint[] TexIDs = [0];
                                                  
            // Meshing                            
            [JsonProperty("Custom_Mesh")]         public string? Custom_Mesh;
                                                  
            // Lighting                           
            [JsonProperty("Light_Emits")]         public byte Light_Emits = 0;
            [JsonProperty("Light_Opaque")]        public bool Light_Opaque = true;
        }
    }
}
