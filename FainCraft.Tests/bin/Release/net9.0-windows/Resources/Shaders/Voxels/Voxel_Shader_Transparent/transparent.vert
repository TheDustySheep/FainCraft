#version 450 core

layout(std430, binding = 0) buffer LightingBuffer { uint lighting[]; };

#include "../common.glsl"
#include "../decode.glsl"
#include "../water.glsl"

layout (location = 0) in int aData1;
layout (location = 1) in int aData2;


out VertexData vData;

uniform mat4 uModel;
uniform mat4 uProjection;
uniform mat4 uView;
uniform float time;

const vec3 BIOME_COLORS[] = vec3[]
(
   vec3(142.0 / 255.0, 179.0 / 255.0, 97.0 / 255.0)
);

const vec3 FOLIAGE_COLOR = vec3(58.0 / 255.0, 95.0 / 255.0, 11.0 / 255.0);

vec3 GetBlendCol(DecodeData dData)
{
    vec3 blendCol = vec3(1, 1, 1);
    
    if (dData.AnimateFoliage && dData.BlendBiome)
    {
        blendCol = (FOLIAGE_COLOR + BIOME_COLORS[dData.BiomeIndex]) * 0.5;
    }
    else if (dData.BlendBiome)
    {
        blendCol = BIOME_COLORS[dData.BiomeIndex];
    }    
    else if (dData.AnimateFoliage)
    {
        blendCol = FOLIAGE_COLOR;
    }
    
    return blendCol;
}

float ApplyFluid(DecodeData dData)
{
    if (!dData.ApplyFluid)
        return 0;

    vec3 globalPos = vec3(uModel * vec4(dData.Position, 1.0));
    //vec3 globalPos = dData.Position;
    //float waveHeight = GetWave(globalPos.xz, time);
    float waveHeight = computeWaveHeight(globalPos.xz, time);

    return waveHeight * 0.2 - 0.2;
}


void main()
{
    DecodeData dData = DecodeVertex(aData1, aData2);

    dData.Position.y += ApplyFluid(dData);

    // Outputs
    gl_Position = uProjection * uView * uModel * vec4(dData.Position, 1.0);

    vData.TexCoord   = dData.TexCoord;
    vData.Normal     = mat3(transpose(inverse(uModel))) * dData.Normal;
    vData.FragPos    = vec3(uModel * vec4(dData.Position, 1.0));
    vData.AO         = dData.AO;
    vData.Light      = float(dData.LightSky) / 31.0;
    vData.BlendColor = GetBlendCol(dData);

}