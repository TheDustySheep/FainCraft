#version 330 core
#extension GL_EXT_texture_array: enable

struct Lighting {
    vec3 direction;
	vec3 specular;
    vec3 ambient;
    vec3 diffuse;
};

out vec4 FragColor;

in vec3 oTexCoord;
in vec3 oVertexNormal;
in vec3 oFragPos;
in float oAO;
in vec3 oBlendColor;

uniform vec3 viewPos;
uniform sampler2DArray albedoTexture;
uniform Lighting lighting;

vec3 fresnelSchlick(vec3 viewDir, vec3 normal, vec3 F0) {
    float cosTheta = clamp(dot(viewDir, normal), 0.0, 1.0);
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

vec3 CalcLighting(vec3 baseDiffColor)
{
    vec3 viewDir = normalize(viewPos - oFragPos);
    vec3 normal = normalize(oVertexNormal);
    vec3 lightDir = normalize(-lighting.direction);

    // Diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);

    // Specular shading
    vec3 reflectDir = reflect(-lightDir, normal);  // reflect expects -lightDir
    float specularStrength = 0.5;
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);

    // Fresnel term (for water, F0 0.02)
    vec3 F0 = vec3(0.02);
    vec3 fresnel = fresnelSchlick(viewDir, normal, F0);

    // Combine lighting components
    vec3 ambient = lighting.ambient * baseDiffColor;
    vec3 diffuse = lighting.diffuse * diff * baseDiffColor;
    vec3 specular = lighting.specular * specularStrength * spec * fresnel;

    vec3 finalColor = ambient + diffuse + specular;

    return finalColor;
}

void main() 
{
    // Vertex Color
    vec4 vertexColor = texture2DArray(albedoTexture, oTexCoord).rgba;

    if (vertexColor.a < 0.1)
        discard;

    vertexColor.rgb *= oAO;
    vertexColor.rgb *= oBlendColor;

    // Result
    //vec3 result = vertexColor.rgb;
    vec3 result = CalcLighting(vertexColor.rgb);
    FragColor = vec4(result, vertexColor.a);
}