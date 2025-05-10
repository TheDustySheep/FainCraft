struct DecodeData 
{
    // Positions
     vec3 Position;
    uvec3 Coord;
    uvec3 Offset;

    // Normals
    ivec3 Normal;
    ivec3 FaceCoord;

    // Textures
    uint Corner;
    vec3 TexCoord;
    
    // Fluid
    bool IsFluid;

    // Light
    uint AO;
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

    dData.Coord = uvec3
    (
        (aData1 >>  0u) & 31u,
        (aData1 >>  5u) & 31u,
        (aData1 >> 10u) & 31u
    );

    dData.Offset = uvec3
    (
        (aData1 >> 17u) & 1u,
        (aData1 >> 16u) & 1u,
        (aData1 >> 15u) & 1u
    );

    dData.Position = vec3(ivec3(dData.Coord) + ivec3(dData.Offset));

    dData.Corner = (aData1 >> 18) & 3;

    dData.Normal = NORMAL_LOOKUP[(aData1 >> 20) & 7];

    // In cluster space
    dData.FaceCoord = ivec3(dData.Coord) + dData.Normal;

    dData.AO = (aData1 >> 24) & 3;

    // Tex coords
    dData.TexCoord = vec3(TEX_UV_LOOKUP[dData.Corner], float(aData2 & 65535));
    
    uint light       = GetLight(dData.FaceCoord);
    dData.LightSky   = light & 15u;
    dData.LightVoxel = (light >> 4) & 15u;

    dData.AnimateFoliage = bool((aData1 >> 26) & 1);
    dData.BlendBiome     = bool((aData1 >> 27) & 1);
    dData.BiomeIndex     =      (aData1 >> 28) & 15;

    return dData;
}