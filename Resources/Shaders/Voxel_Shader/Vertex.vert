#version 330 core
layout (location = 0) in int aData1;
layout (location = 1) in int aData2;

out vec2 texCoord;
out vec3 vertexNormal;
out vec3 fragPos;

uniform mat4 normalMat;
uniform mat4 uModel;
uniform mat4 uProjection;
uniform mat4 uView;

struct Vertex_Data
{
    vec3 Position;
    vec3 Normal;
    int corner;
    int texID;
    vec2 TexCoord;
};

const vec3 NORMAL_LOOKUP[6] = vec3[]
(
   vec3(-1, 0, 0), 
   vec3( 1, 0, 0), 
   vec3( 0,-1, 0), 
   vec3( 0, 1, 0), 
   vec3( 0, 0,-1), 
   vec3( 0, 0, 1) 
);

const vec2 TEX_LOOKUP[4] = vec2[]
(
   vec2(0, 1), 
   vec2(1, 1), 
   vec2(1, 0), 
   vec2(0, 0)
);

vec2 DecodeTexCoord(int index, int corner)
{
    int xIdx = (index & 15);
    int yIdx = (index >> 4) & 15;
    
    vec2 coord = vec2(xIdx, yIdx);

    switch (corner) 
    {
        case 0:
            coord += vec2(0, 0);
            break;
        case 1:
            coord += vec2(0, 1);
            break;
        case 2:
            coord += vec2(1, 1);
            break;
        default:
            coord += vec2(1, 0);
            break;
    }

    return coord / 16;
}

Vertex_Data DecodeVertex()
{
    Vertex_Data vertex_data;

    vertex_data.Position.x = float(uint((aData1 >>  0) & 63));
    vertex_data.Position.y = float(uint((aData1 >>  6) & 63));
    vertex_data.Position.z = float(uint((aData1 >> 12) & 63));

    vertex_data.Normal = NORMAL_LOOKUP[(aData1 >> 20) & 7];

    vertex_data.corner = (aData1 >> 18) & 3;
    vertex_data.texID = aData2 & 65535;

    vertex_data.TexCoord = DecodeTexCoord(vertex_data.texID ,vertex_data.corner);

    return vertex_data;
}

void main()
{
    Vertex_Data vertex_data = DecodeVertex();

    vec3 aPosition = vertex_data.Position;
    vec3 aNormal   = vertex_data.Normal;
    vec2 aTexCoord = vertex_data.TexCoord;

    // Outputs
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
    fragPos = vec3(uModel * vec4(aPosition, 1.0));
    vertexNormal = mat3(normalMat) * aNormal;
    texCoord = aTexCoord;
}