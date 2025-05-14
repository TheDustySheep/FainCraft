struct MeshFace {
	vec3 VertA;
	vec3 VertB;
	vec3 VertC;
	vec3 VertD;

	vec3 Normal;
	uint FaceCoord;
};

layout(std430, binding = 0) buffer MeshBuffer { MeshFace meshFaces[]; };

vec3 LookupFaceVert(MeshFace face, uint corner)
{
	switch (corner) 
	{
		case 0:
			return face.VertA;
		case 1:
			return face.VertB;
		case 2:
			return face.VertC;
		case 3:
			return face.VertD;
		default:
			return vec3(0);
	}
}