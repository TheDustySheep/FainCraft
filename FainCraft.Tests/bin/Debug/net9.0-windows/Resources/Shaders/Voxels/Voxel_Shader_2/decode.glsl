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
    bool isFluid;

    // Light
    uint AO;
    uint LightSky;
    uint LightVoxel;

    // Biome
    int biomeIndex;
    bool blendBiome;

    // Foliage
    bool animateFoliage;
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
        (aData1 >>  0) & 31,
        (aData1 >>  5) & 31,
        (aData1 >> 10) & 31
    );

    dData.Offset = uvec3
    (
        (aData1 >> 17) & 1,
        (aData1 >> 16) & 1,
        (aData1 >> 15) & 1
    );

    dData.Position = vec3(dData.Coord + dData.Offset);

    dData.Corner = (aData1 >> 18) & 3;

    dData.Normal = NORMAL_LOOKUP[(aData1 >> 20) & 7];

    // In cluster space
    dData.FaceCoord = ivec3(dData.Coord) + dData.Normal;

    dData.AO = (aData1 >> 24) & 3;

    // Tex coords
    dData.TexCoord = vec3(TEX_UV_LOOKUP[dData.Corner], float(aData2 & 65535));
    
    uint light     = GetLight(dData.FaceCoord);
    dData.LightSky = light & 31u;

    //dData.LightSky   = 31;
    //dData.LightVoxel = 31;
    //dData.isFluid = bool((aData1 >> 23) & 1);
    //
    //
    //
    //dData.texID = aData2 & 65535;
    //
    //dData.TexCoord = DecodeTexCoord(dData.texID, dData.corner);
    //dData.AO = (aData1 >> 24) & 3;
    //
    //dData.animateFoliage = bool((aData1 >> 26) & 1);
    //dData.blendBiome = bool((aData1 >> 27) & 1);
    //dData.biomeIndex = (aData1 >> 28) & 15;

    return dData;
}