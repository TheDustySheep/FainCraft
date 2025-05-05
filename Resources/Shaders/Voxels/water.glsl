#include "Resources/ShaderUtils/Lighting.glsl"

const float waveSpeed      = 1.0;
const float waveScale      = 0.5;
const float waveHeight     = 0.2;
const float specularPower  = 32.0;
const float fresnelBias    = 0.1;
const float fresnelScale   = 0.9;
const float fresnelPower   = 20.0;
const float depthFadeScale = 10.0;

float computeWaveHeight(vec3 pos, float time) {
    float wave1 = sin(pos.x * 0.1 + time * waveSpeed);
    float wave2 = sin(pos.z * 0.15 + time * waveSpeed * 1.2);
    float wave3 = sin((pos.x + pos.z) * 0.07 + time * waveSpeed * 0.8);
    return (wave1 + wave2 + wave3) * waveHeight * 0.333;
}

vec4 shadeWater(
    vec3  pos,
    vec3  viewDir,
    vec3  albedo,
    float depthDiff,
    float time
) {
    float eps = 0.1;
    float hC = computeWaveHeight(pos, time);
    float hX = computeWaveHeight(pos + vec3(eps, 0.0, 0.0), time);
    float hZ = computeWaveHeight(pos + vec3(0.0, 0.0, eps), time);
    vec3 normal = normalize(vec3((hX - hC)/eps, 1.0, (hZ - hC)/eps));

    float NdotL   = max(dot(normal, -light.direction), 0.0);
    vec3  diffuse = light.diffuse * NdotL;
    vec3  view    = normalize(viewDir);
    vec3  reflectDir = reflect(light.direction, normal);
    float specAngle = max(dot(view, reflectDir), 0.0);
    float spec = pow(specAngle, specularPower);
    vec3  specular = light.specular * spec;

    float baseF = 1.0 - max(dot(normal, view), 0.0);
    float fresnel = fresnelBias + fresnelScale * pow(baseF, fresnelPower);
    fresnel = clamp(fresnel, 0.0, 1.0);

    vec3 horizonColor = vec3(0.1, 0.2, 0.4);
    vec3 color = albedo * (light.ambient + diffuse * light.intensity)
                 + specular * light.intensity;
    color = mix(color, horizonColor, fresnel);

    float depthFade = clamp(depthDiff * depthFadeScale, 0.0, 1.0);

    float alphaFresnel = mix(0.2, 1.0, fresnel);
    float alpha = alphaFresnel * depthFade;

    return vec4(color, alpha);
}
