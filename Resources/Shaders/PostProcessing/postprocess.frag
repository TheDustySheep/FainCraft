#version 330 core

out vec4 FragColor;
in vec2 texCoords;

uniform sampler2D colorTexture;
uniform sampler2D depthTexture;

uniform float fogStart = 128.0;
uniform float fogEnd = 256.0;
uniform vec4 fogColor = vec4(0.623, 0.734, 0.785, 0);

uniform float zFar = 10000.0;
uniform float zNear = 0.1;

float linearize_depth(float d)
{
    float z_n = 2.0 * d - 1.0;
    return 2.0 * zNear * zFar / (zFar + zNear - z_n * (zFar - zNear));
}

float InvLerp(float a, float b, float v) 
{
    return (v - a) / (b - a);
}

float Remap(float iMin, float iMax, float oMin, float oMax, float v) 
{
    return mix(oMin, oMax, InvLerp(iMin, iMax, v));
}

float calculateFogFactor(float d) 
{
    float density = 0.002;
    float result = 1.0 - exp(-pow(density * d, 2.0));
    return clamp(result, 0, 1);
}

void main()
{
    float depthCol = texture(depthTexture, texCoords).r;
    vec4 pixelCol = texture(colorTexture, texCoords);
    
    // Sky
    if (depthCol == 1)
    {
        FragColor = pixelCol;
    }
    // Anything else
    else
    {
        float linDepth = linearize_depth(depthCol);
        float t = calculateFogFactor(linDepth);
        FragColor = mix(pixelCol, fogColor, t);
    }
}