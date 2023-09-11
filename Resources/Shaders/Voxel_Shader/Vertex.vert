#version 330 core
layout (location = 0) in int aData1;
layout (location = 1) in int aData2;

out vec3 oTexCoord;
out vec3 oVertexNormal;
out vec3 oFragPos;
out float oAO;

uniform mat4 normalMat;
uniform mat4 uModel;
uniform mat4 uProjection;
uniform mat4 uView;
uniform float time;

struct Vertex_Data
{
    vec3 Position;
    vec3 Normal;
    int corner;
    int texID;
    vec3 TexCoord;
    bool isFluid;
    int AO;
};

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
    //int xIdx = (index & 15);
    //int yIdx = (index >> 4) & 15;
    
    vec3 coord = vec3(0, 0, index);

    switch (corner) 
    {
        case 0:
            coord.xy = vec2(0, 0);
            break;
        case 1:
            coord.xy = vec2(0, 1);
            break;
        case 2:
            coord.xy = vec2(1, 1);
            break;
        default:
            coord.xy = vec2(1, 0);
            break;
    }

    return coord;
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

float GetWave(vec2 pos)
{
    float xWaveLastDir = GetSumOfCoSins(pos.x + (time - 1) * 2.5, 0.1);
    float zWaveLastDir = GetSumOfCoSins(pos.x + (time - 1) * 3.2, 0.1);

    float xWave = GetSumOfSins(pos.x + xWaveLastDir + time * 2.5, 0.1);
    float zWave = GetSumOfSins(pos.y + zWaveLastDir + time * 3.2, 0.1);

    return (xWave + zWave) * 0.5 * 0.2;
}

Vertex_Data ApplyFluid(Vertex_Data vertex_data)
{
    if (!vertex_data.isFluid)
        return vertex_data;

    if (vertex_data.Normal.y == 1.0 || 
       (vertex_data.Normal.y != -1.0 && (vertex_data.corner == 1 || vertex_data.corner == 2)))
    {
        vec3 globalPos = vec3(uModel * vec4(vertex_data.Position, 1.0));

        float wavePos = GetWave(globalPos.xz);
        vertex_data.Position.y += -0.2 + wavePos;
    }

    return vertex_data;
}

float DecodeAO(int ao)
{
    return (float(ao) + 1.0) * 0.25;
}

Vertex_Data DecodeVertex()
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

    vertex_data = ApplyFluid(vertex_data);

    return vertex_data;
}

void main()
{
    Vertex_Data vertex_data = DecodeVertex();

    vec3 aPosition = vertex_data.Position;
    vec3 aNormal   = vertex_data.Normal;
    vec3 aTexCoord = vertex_data.TexCoord;

    // Outputs
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
    oFragPos = vec3(uModel * vec4(aPosition, 1.0));
    oVertexNormal = mat3(normalMat) * aNormal;
    oTexCoord = aTexCoord;
    oAO = DecodeAO(vertex_data.AO);
}