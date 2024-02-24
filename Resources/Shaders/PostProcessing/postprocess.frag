#version 330 core

out vec4 FragColor;
in vec2 texCoords;

uniform sampler2D screenTexture;

void main()
{
    float depth = gl_FragDepth;
    //FragColor = vec4(depth);
    FragColor = texture(screenTexture, texCoords);
}