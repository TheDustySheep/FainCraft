#version 330 core

layout (location = 0) in vec2 inPos;
layout (location = 1) in vec2 inTexCoords;
layout (location = 2) in float inDepth;

out vec2 texCoords;

void main()
{
    gl_Position = vec4(inPos.x, inPos.y, inDepth, 1.0); 
    texCoords = inTexCoords;
}  