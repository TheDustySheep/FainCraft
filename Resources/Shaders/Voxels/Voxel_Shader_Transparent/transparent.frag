#version 330 core
#extension GL_EXT_texture_array: enable

#include "../water.glsl"

out vec4 FragColor;

in vec3 oTexCoord;
in vec3 oVertexNormal;
in vec3 oFragPos;
in float oAO;
in vec3 oBlendColor;

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
    vec4 vertexColor = texture2DArray(albedoTexture, oTexCoord).rgba;

    //vertexColor.rgb *= oBlendColor;
    float sceneDepth = linearizeDepth(texture(depthTexture, gl_FragCoord.xy / screenSize).r);
    float fragDepth = linearizeDepth(gl_FragCoord.z);
    float depthDiff = sceneDepth - fragDepth;

    // Result
    vec3 normal = normalize(oVertexNormal);
    vec3 viewDir = normalize(viewPos - oFragPos);
    vec4 result = shadeWater(oFragPos, viewDir, vertexColor.rgb, depthDiff, time);
    
    result.rgb *= oAO;
    result.a *= 2;
    result.a = clamp(result.a, 0.0, 1.0);
    //FragColor = vec4(vec3(gl_FragCoord.z), 1.0);

    FragColor = result;
}