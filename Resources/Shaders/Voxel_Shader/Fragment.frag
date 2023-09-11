#version 330 core
#extension GL_EXT_texture_array: enable

struct Lighting {
    vec3 direction;
	
    vec3 ambient;
    vec3 diffuse;
};

out vec4 FragColor;

in vec3 oTexCoord;
in vec3 oVertexNormal;
in vec3 oFragPos;
in float oAO;

uniform vec3 viewPos;
uniform sampler2DArray albedoTexture;
uniform Lighting lighting;

vec3 CalcLighting(Lighting light, vec3 baseDiffColor)
{
    vec3 viewDir = normalize(viewPos - oFragPos);

    vec3 lightDir = normalize(-light.direction);
    // diffuse shading
    float diff = max(dot(oVertexNormal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, oVertexNormal);
    // combine results
    vec3 ambient = light.ambient * baseDiffColor;
    vec3 diffuse = light.diffuse * diff * baseDiffColor;
    return (ambient + diffuse);
}

void main() 
{
    // Vertex Color
    vec4 vertexColor = texture2DArray(albedoTexture, oTexCoord).rgba;

    if (vertexColor.a < 0.1)
        discard;

    vertexColor.rgb *= oAO;

    // Result
    vec3 result = vertexColor.rgb;
    //vec3 result = CalcLighting(lighting, vertexColor.rgb);
    FragColor = vec4(result, vertexColor.a);
}