struct MeshQuad {
	float VertA_X;
	float VertA_Y;
	float VertA_Z;

	float VertB_X;
	float VertB_Y;
	float VertB_Z;

	float VertC_X;
	float VertC_Y;
	float VertC_Z;

	float VertD_X;
	float VertD_Y;
	float VertD_Z;

	float Normal_X;
	float Normal_Y;
	float Normal_Z;

	float UVMin_X;
	float UVMin_Y;

	float UVMax_X;
	float UVMax_Y;

	uint FaceCoord;
};

struct MeshVert {
	vec3 Position;
	vec3 Normal;
	vec2 UV;
	ivec3 FaceCoord;
};

layout(std430, binding = 0) buffer MeshBuffer { MeshQuad meshFaces[]; };

const ivec3 NORMAL_LOOKUP[6] = ivec3[]
(
   ivec3(-1, 0, 0), 
   ivec3( 1, 0, 0), 
   ivec3( 0,-1, 0), 
   ivec3( 0, 1, 0), 
   ivec3( 0, 0,-1), 
   ivec3( 0, 0, 1) 
);

MeshVert GetMeshVert(uint meshIndex, uint corner)
{
	MeshQuad quad = meshFaces[meshIndex];

	MeshVert vert;

	switch (corner) 
	{
		case 0: vert.Position = vec3(quad.VertA_X, quad.VertA_Y, quad.VertA_Z); break;
		case 1: vert.Position = vec3(quad.VertB_X, quad.VertB_Y, quad.VertB_Z); break;
		case 2: vert.Position = vec3(quad.VertC_X, quad.VertC_Y, quad.VertC_Z); break;
		case 3: vert.Position = vec3(quad.VertD_X, quad.VertD_Y, quad.VertD_Z); break;
		default: break;
	}

	switch (corner) 
	{
		case 0: vert.UV = vec2(quad.UVMin_X, quad.UVMin_Y); break;
		case 1: vert.UV = vec2(quad.UVMin_X, quad.UVMax_Y); break;
		case 2: vert.UV = vec2(quad.UVMax_X, quad.UVMax_Y); break;
		case 3: vert.UV = vec2(quad.UVMax_X, quad.UVMin_Y); break;
		default: break;
	}

	vert.Normal = vec3(quad.Normal_X, quad.Normal_Y, quad.Normal_Z);

	vert.FaceCoord = NORMAL_LOOKUP[quad.FaceCoord];

	return vert;
}