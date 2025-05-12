#version 450 core
#extension GL_EXT_texture_array: enable

#include "../common.glsl"
#include "Resources/ShaderUtils/Lighting.glsl"

out vec4 FragColor;

in VertexData vData;

uniform vec3 viewPos;
uniform sampler2DArray albedoTexture;

void main() 
{
    // Vertex Color
    vec4 color = texture(albedoTexture, vData.TexCoord).rgba;

    if (color.a < 0.1)
        discard;

    color.rgb *= vData.Light;
    color.rgb *= vData.BlendColor;
    
    // Result
    //vec3 normal = normalize(oVertexNormal);
    //vec3 viewDir = normalize(viewPos - oFragPos);
    
    //color.rgb = CalcLighting(color.rgb, viewDir, normal);
    
    //color.rgb *= oVoxelLight;

    color.rgb *= 1 - pow(1 - vData.AO, 6);
    
    FragColor = color;
}