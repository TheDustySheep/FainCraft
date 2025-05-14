#include "./custom_meshes.glsl"

struct DecodeData 
{
    // Positions
    ivec3 Coord;
    vec3 Position;

    // Normals
    vec3 Normal;
    ivec3 FaceCoord;

    // Textures
    uint MeshIndex;
    uint Corner;
    vec3 TexCoord;
    
    // Fluid
    bool ApplyFluid;

    // Light
    float AO;
    uint LightSky;
    uint LightVoxel;

    // Biome
    int  BiomeIndex;
    bool BlendBiome;

    // Foliage
    bool AnimateFoliage;
};

// Lookups
const ivec3 NORMAL_LOOKUP[6] = ivec3[]
(
   ivec3(-1, 0, 0), 
   ivec3( 1, 0, 0), 
   ivec3( 0,-1, 0), 
   ivec3( 0, 1, 0), 
   ivec3( 0, 0,-1), 
   ivec3( 0, 0, 1) 
);

const vec2 TEX_UV_LOOKUP[4] = vec2[]
(
    vec2(0.0, 0.0),
    vec2(0.0, 1.0),
    vec2(1.0, 1.0),
    vec2(1.0, 0.0)
);

uint GetLight(ivec3 coord) {
    uint index = 
        (uint(coord.x + 1)) + 
        (uint(coord.z + 1) * 34u) + 
        (uint(coord.y + 1) * 34u * 34u);

    return lighting[index];
}

DecodeData DecodeVertex(int aData1, int aData2)
{
    DecodeData dData;

    // Basic Data Extraction
    dData.Corner    = (aData1 >> 16) & 3;
    dData.MeshIndex = (aData2 >> 16) & 65535;
    dData.AO        = float((aData1 >> 24) & 3) * 0.25 + 0.25;

    // Lookup the mesh face
    MeshFace meshFace = meshFaces[dData.MeshIndex];

    // Normal lookup from face
    dData.Normal = meshFace.Normal;

    dData.Coord = ivec3
    (
        (aData1 >>  0u) & 31,
        (aData1 >>  5u) & 31,
        (aData1 >> 10u) & 31
    );

    dData.Position = vec3(dData.Coord) + LookupFaceVert(meshFace, dData.Corner);

    // In cluster space
    dData.FaceCoord = dData.Coord + NORMAL_LOOKUP[meshFace.FaceCoord];
        
    // Tex coords
    dData.TexCoord  = vec3(TEX_UV_LOOKUP[dData.Corner], float(aData2 & 65535));
    
    // Lighting
    uint light       = GetLight(dData.FaceCoord);
    dData.LightSky   = light & 15u;
    dData.LightVoxel = (light >> 4) & 15u;

    dData.AnimateFoliage = bool((aData1 >> 26) & 1);
    dData.BlendBiome     = bool((aData1 >> 27) & 1);
    dData.BiomeIndex     =      (aData1 >> 28) & 15;

    return dData;
}