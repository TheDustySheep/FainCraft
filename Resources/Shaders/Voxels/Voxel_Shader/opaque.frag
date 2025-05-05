#version 330 core
#extension GL_EXT_texture_array: enable

#include "Resources/ShaderUtils/Lighting.glsl"

out vec4 FragColor;

in vec3 oTexCoord;
in vec3 oVertexNormal;
in vec3 oFragPos;
in float oAO;
in vec3 oBlendColor;

uniform vec3 viewPos;
uniform sampler2DArray albedoTexture;

void main() 
{
    // Vertex Color
    vec4 color = texture2DArray(albedoTexture, oTexCoord).rgba;

    if (color.a < 0.1)
        discard;

    color.rgb *= oBlendColor;
    
    // Result
    vec3 normal = normalize(oVertexNormal);
    vec3 viewDir = normalize(viewPos - oFragPos);
    
    color.rgb = CalcLighting(color.rgb, viewDir, normal);
    
    color.rgb *= oAO;
    
    FragColor = color;
}