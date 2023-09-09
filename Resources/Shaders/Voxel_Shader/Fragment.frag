#version 330 core
struct Lighting {
    vec3 direction;
	
    vec3 ambient;
    vec3 diffuse;
};

out vec4 FragColor;

in vec2 texCoord;
in vec3 vertexNormal;
in vec3 fragPos;  

uniform vec3 viewPos;
uniform sampler2D albedoTexture;
uniform Lighting lighting;

vec3 CalcLighting(Lighting light, vec3 baseDiffColor)
{
    vec3 viewDir = normalize(viewPos - fragPos);

    vec3 lightDir = normalize(-light.direction);
    // diffuse shading
    float diff = max(dot(vertexNormal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, vertexNormal);
    // combine results
    vec3 ambient = light.ambient * baseDiffColor;
    vec3 diffuse = light.diffuse * diff * baseDiffColor;
    return (ambient + diffuse);
}

void main() 
{
    // Vertex Color
    vec4 vertexColor = texture(albedoTexture, texCoord).rgba;

    if (vertexColor.a < 0.1)
        discard;

    // Result
    vec3 result = vertexColor.rgb;
    //vec3 result = CalcLighting(lighting, vertexColor.rgb);
    FragColor = vec4(result, vertexColor.a);
}