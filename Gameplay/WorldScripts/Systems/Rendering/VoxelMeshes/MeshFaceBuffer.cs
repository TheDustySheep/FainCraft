using FainEngine_v2.Rendering.Meshing;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes
{
    public class MeshFaceBuffer : IDisposable
    {
        ShaderStorageBufferObject<MeshFace> _ssbo;

        private readonly MeshFace[] meshFaces;

        public MeshFaceBuffer()
        {
            meshFaces = StandardFaces;

            _ssbo = new((uint)meshFaces.Length);
            _ssbo.SetData(meshFaces);
        }

        private static readonly MeshFace[] StandardFaces =
        {
            // X-
            new MeshFace()
            {
                VertA     = new( 0, 0, 1),
                VertB     = new( 0, 1, 1),
                VertC     = new( 0, 1, 0),
                VertD     = new( 0, 0, 0),
                Normal    = new(-1, 0, 0),
                FaceCoord = 0,
            },
            // X+
            new MeshFace()
            {
                VertA     = new( 1, 0, 0),
                VertB     = new( 1, 1, 0),
                VertC     = new( 1, 1, 1),
                VertD     = new( 1, 0, 1),
                Normal    = new( 1, 1, 0),
                FaceCoord = 1,
            },
            // Y-
            new MeshFace()
            {
                VertA     = new( 1, 0, 0),
                VertB     = new( 1, 0, 1),
                VertC     = new( 0, 0, 1),
                VertD     = new( 0, 0, 0),
                Normal    = new( 0,-1, 0),
                FaceCoord = 2,
            },
            // Y+
            new MeshFace()
            {

                VertA     = new( 0, 1, 0),
                VertB     = new( 0, 1, 1),
                VertC     = new( 1, 1, 1),
                VertD     = new( 1, 1, 0),
                Normal    = new( 0, 1, 0),
                FaceCoord = 3,
            },
            // Z-
            new MeshFace()
            {
                VertA     = new( 0, 0, 0),
                VertB     = new( 0, 1, 0),
                VertC     = new( 1, 1, 0),
                VertD     = new( 1, 0, 0),
                Normal    = new( 0, 0,-1),
                FaceCoord = 4,
            },
            // Z+
            new MeshFace()
            {
                VertA     = new( 1, 0, 1),
                VertB     = new( 1, 1, 1),
                VertC     = new( 0, 1, 1),
                VertD     = new( 0, 0, 1),
                Normal    = new( 0, 0, 1),
                FaceCoord = 5,
            }
        };

        public void Dispose()
        {
            _ssbo.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Bind(uint bindingPoint)
        {
            _ssbo.Bind(bindingPoint);
        }

        ~MeshFaceBuffer() => Dispose();
    }
}
