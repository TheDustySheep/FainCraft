#version 330 core

#include "../vert_utils.glsl"

layout (location = 0) in int aData1;
layout (location = 1) in int aData2;

out vec3 oTexCoord;
out vec3 oVertexNormal;
out vec3 oFragPos;
out float oAO;
out vec3 oBlendColor;

uniform mat4 uModel;
uniform mat4 uProjection;
uniform mat4 uView;
uniform float time;

const vec3 BIOME_COLORS[] = vec3[]
(
   vec3(142.0 / 255.0, 179.0 / 255.0, 97.0 / 255.0)
);

const vec3 FOLIAGE_COLOR = vec3(58.0 / 255.0, 95.0 / 255.0, 11.0 / 255.0);

void main()
{
    Vertex_Data vertex_data = DecodeVertex(aData1, aData2, uModel, time);

    vec3 aPosition = vertex_data.Position;
    vec3 aNormal   = vertex_data.Normal;
    vec3 aTexCoord = vertex_data.TexCoord;

    // Outputs
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
    oFragPos = vec3(uModel * vec4(aPosition, 1.0));

    oVertexNormal = mat3(transpose(inverse(uModel))) * aNormal;  // to world space

    oTexCoord = aTexCoord;
    oAO = DecodeAO(vertex_data.AO);

    vec3 blendCol = vec3(1, 1, 1);

    if (vertex_data.animateFoliage && vertex_data.blendBiome)
    {
        blendCol = (FOLIAGE_COLOR + BIOME_COLORS[vertex_data.biomeIndex]) * 0.5;
    }
    else if (vertex_data.blendBiome)
    {
        blendCol = BIOME_COLORS[vertex_data.biomeIndex];
    }    
    else if (vertex_data.animateFoliage)
    {
        blendCol = FOLIAGE_COLOR;
    }

    oBlendColor = blendCol;
}