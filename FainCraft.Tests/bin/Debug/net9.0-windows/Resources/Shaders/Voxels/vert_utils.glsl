#include "./water.glsl"

// Structs
struct Vertex_Data
{
    vec3 Position;
    vec3 Normal;
    int corner;
    int texID;
    vec3 TexCoord;
    bool isFluid;
    int AO;
    int biomeIndex;
    bool blendBiome;
    bool animateFoliage;
};

// Lookups
const vec3 NORMAL_LOOKUP[6] = vec3[]
(
   vec3(-1, 0, 0), 
   vec3( 1, 0, 0), 
   vec3( 0,-1, 0), 
   vec3( 0, 1, 0), 
   vec3( 0, 0,-1), 
   vec3( 0, 0, 1) 
);

const vec2 TEX_LOOKUP[4] = vec2[]
(
   vec2(0, 1), 
   vec2(1, 1), 
   vec2(1, 0), 
   vec2(0, 0)
);

vec3 DecodeTexCoord(int index, int corner)
{
    const vec2 offsets[4] = vec2[4](
        vec2(0.0, 0.0),
        vec2(0.0, 1.0),
        vec2(1.0, 1.0),
        vec2(1.0, 0.0)
    );

    vec2 uv = offsets[corner];
    return vec3(uv, index);
}

float DecodeAO(int ao)
{
    return (float(ao) + 1.0) * 0.25;
}

float GetSumOfCoSins(float t, float frequency)
{    
    float amplitude = 1;
    float result = 0;

    for (int i = 0; i < 4; i++)
    {
        amplitude *= 0.5;
        frequency *= 2;
        result += amplitude * cos(frequency * t);
    }
    return result / 1.875;
}

float GetSumOfSins(float t, float frequency)
{    
    float amplitude = 1;
    float result = 0;

    for (int i = 0; i < 4; i++)
    {
        amplitude *= 0.5;
        frequency *= 2;
        result += amplitude * sin(frequency * t);
    }
    return result / 1.875;
}

float GetWave(vec2 pos, float time)
{
    float xWaveLastDir = GetSumOfCoSins(pos.x + (time - 1) * 2.5, 0.1);
    float zWaveLastDir = GetSumOfCoSins(pos.x + (time - 1) * 3.2, 0.1);

    float xWave = GetSumOfSins(pos.x + xWaveLastDir + time * 2.5, 0.1);
    float zWave = GetSumOfSins(pos.y + zWaveLastDir + time * 3.2, 0.1);

    return (xWave + zWave) * 0.5 * 0.2;
}

Vertex_Data ApplyFluid(Vertex_Data vertex_data, mat4x4 uModel, float time)
{
    if (!vertex_data.isFluid)
        return vertex_data;

    if (vertex_data.Normal.y == 1.0 || 
       (vertex_data.Normal.y != -1.0 && (vertex_data.corner == 1 || vertex_data.corner == 2)))
    {
        vec3 globalPos = vec3(uModel * vec4(vertex_data.Position, 1.0));

        //float waveHeight = GetWave(globalPos.xz, time);
        float waveHeight = computeWaveHeight(globalPos, time);
        vertex_data.Position.y += waveHeight - 0.2;
    }

    return vertex_data;
}

Vertex_Data DecodeVertex(int aData1, int aData2, mat4x4 uModel, float time)
{
    Vertex_Data vertex_data;

    vertex_data.isFluid = bool((aData1 >> 23) & 1);

    vertex_data.Position = vec3
    (
        float(uint((aData1 >>  0) & 63)),
        float(uint((aData1 >>  6) & 63)),
        float(uint((aData1 >> 12) & 63))
    );

    vertex_data.Normal = NORMAL_LOOKUP[(aData1 >> 20) & 7];

    vertex_data.corner = (aData1 >> 18) & 3;
    vertex_data.texID = aData2 & 65535;

    vertex_data.TexCoord = DecodeTexCoord(vertex_data.texID ,vertex_data.corner);
    vertex_data.AO = (aData1 >> 24) & 3;

    vertex_data.animateFoliage = bool((aData1 >> 26) & 1);
    vertex_data.blendBiome = bool((aData1 >> 27) & 1);
    vertex_data.biomeIndex = (aData1 >> 28) & 15;

    vertex_data = ApplyFluid(vertex_data, uModel, time);

    return vertex_data;
}