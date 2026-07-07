#version 330 core

out vec4 FragColor;

in vec2 texCoords;

uniform sampler2D albedoTexture;

void main()
{
    FragColor = texture(albedoTexture, texCoords);
    //FragColor = vec4(1.0, 0.0, 0.0, 0.0);
}