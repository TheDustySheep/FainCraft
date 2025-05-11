#version 330 core
#extension GL_EXT_texture_array: enable

#include "../common.glsl"
#include "../water.glsl"

out vec4 FragColor;

in VertexData vData;

uniform float cam_near;
uniform float cam_far;
uniform float time;
uniform vec3 viewPos;
uniform vec2 screenSize;
uniform sampler2DArray albedoTexture;
uniform sampler2D depthTexture;

float linearizeDepth(float depth)
{
    float z = depth * 2.0 - 1.0;   // Convert from [0, 1] to [-1, 1] range (NDC)
    return (2.0 * cam_near * cam_far) / (cam_far + cam_near - z * (cam_far - cam_near));
}

void main() 
{
    // Vertex Color
    vec4 vertexColor = texture(albedoTexture, vData.TexCoord).rgba;

    //vertexColor.rgb *= oBlendColor;
    float sceneDepth = linearizeDepth(texture(depthTexture, gl_FragCoord.xy / screenSize).r);
    float fragDepth  = linearizeDepth(gl_FragCoord.z);
    float depthDiff  = sceneDepth - fragDepth;

    // Result
    vec3 normal  = normalize(vData.Normal);
    vec3 viewDir = normalize(viewPos - vData.FragPos);
    vec4 result  = shadeWater(vData.FragPos, viewDir, vertexColor.rgb, depthDiff, time);

    FragColor = result;
}