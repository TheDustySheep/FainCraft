#include "Resources/ShaderUtils/Lighting.glsl"

const float waveSpeed      =  1.0;
const float waveScale      =  0.5;
const float specularPower  = 64.0;   
const float fresnelBias    =  0.02;  
const float fresnelScale   =  1.0;
const float fresnelPower   =  5.0;
const float depthFadeScale =  5.0;   

// Improved wave with more layers
float computeWaveHeight(vec2 pos, float time) {
    float w1 = sin(pos.x * 0.1 + time * waveSpeed);
    float w2 = sin(pos.y * 0.15 + time * waveSpeed * 1.3);
    float w3 = sin((pos.x + pos.y) * 0.07 + time * waveSpeed * 0.8);
    float w4 = sin((pos.x * 0.2 - pos.y * 0.1) + time * waveSpeed * 1.7);
    return w1 + w2 + w3 + w4;
}

// Compute normal via central differences
vec3 computeNormal(vec3 pos, float time) {
    float eps = 0.05;  // smaller for smoother normal
    float hL = computeWaveHeight(pos.xz - vec2(eps, 0.0), time);
    float hR = computeWaveHeight(pos.xz + vec2(eps, 0.0), time);
    float hD = computeWaveHeight(pos.xz - vec2(0.0, eps), time);
    float hU = computeWaveHeight(pos.xz + vec2(0.0, eps), time);
    vec3 n = normalize(vec3(hL - hR, 2.0 * eps, hD - hU));
    return n;
}

vec4 shadeWater(
    vec3  pos,
    vec3  viewDir,
    vec3  albedo,
    float depthDiff,
    float time
) {
    vec3 normal = computeNormal(pos, time);

    vec3 L = normalize(-light.direction);
    float NdotL = max(dot(normal, L), 0.0);
    vec3 diffuse = light.diffuse * NdotL;

    vec3 V = normalize(viewDir);
    vec3 R = reflect(-L, normal);
    float specAngle = max(dot(V, R), 0.0);
    float spec = pow(specAngle, specularPower);
    vec3 specular = light.specular * spec;

    // Fresnel - Schlick approx
    float baseF = 1.0 - max(dot(normal, V), 0.0);
    float fresnel = fresnelBias + fresnelScale * pow(baseF, fresnelPower);
    fresnel = clamp(fresnel, 0.0, 1.0);

    vec3 horizonColor = vec3(0.1, 0.2, 0.4);
    vec3 reflectionColor = mix(horizonColor, vec3(0.5, 0.7, 0.9), fresnel);

    vec3 color = albedo * (light.ambient + diffuse * light.intensity)
                 + specular * light.intensity;
    color = mix(color, reflectionColor, fresnel);

    float depthFade = clamp(depthDiff * depthFadeScale, 0.0, 1.0);

    float alpha = mix(0.3, 1.0, fresnel) * depthFade;

    return vec4(color, alpha);
}
