using FainEngine_v2.Rendering.Meshing;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.VoxelMeshes
{
    public class MeshFaceBuffer : IDisposable
    {
        ShaderStorageBufferObject<MeshQuad> _ssbo;

        private readonly MeshQuad[] meshFaces;

        public MeshFaceBuffer(MeshQuad[] customQuads)
        {
            meshFaces = StandardFaces.Concat(customQuads).ToArray();

            _ssbo = new((uint)meshFaces.Length);
            _ssbo.SetData(meshFaces);
        }

        private static readonly MeshQuad[] StandardFaces =
        {
            // X-
            new MeshQuad()
            {
                VertA     = new( 0, 0, 1),
                VertB     = new( 0, 1, 1),
                VertC     = new( 0, 1, 0),
                VertD     = new( 0, 0, 0),
                Normal    = new(-1, 0, 0),
                UVMin     = new( 0, 0),
                UVMax     = new( 1, 1),
                FaceCoord = 0,
            },
            // X+
            new MeshQuad()
            {
                VertA     = new( 1, 0, 0),
                VertB     = new( 1, 1, 0),
                VertC     = new( 1, 1, 1),
                VertD     = new( 1, 0, 1),
                Normal    = new( 1, 1, 0),
                UVMin     = new( 0, 0),
                UVMax     = new( 1, 1),
                FaceCoord = 1,
            },
            // Y-
            new MeshQuad()
            {
                VertA     = new( 1, 0, 0),
                VertB     = new( 1, 0, 1),
                VertC     = new( 0, 0, 1),
                VertD     = new( 0, 0, 0),
                Normal    = new( 0,-1, 0),
                UVMin     = new( 0, 0),
                UVMax     = new( 1, 1),
                FaceCoord = 2,
            },
            // Y+
            new MeshQuad()
            {

                VertA     = new( 0, 1, 0),
                VertB     = new( 0, 1, 1),
                VertC     = new( 1, 1, 1),
                VertD     = new( 1, 1, 0),
                Normal    = new( 0, 1, 0),
                UVMin     = new( 0, 0),
                UVMax     = new( 1, 1),
                FaceCoord = 3,
            },
            // Z-
            new MeshQuad()
            {
                VertA     = new( 0, 0, 0),
                VertB     = new( 0, 1, 0),
                VertC     = new( 1, 1, 0),
                VertD     = new( 1, 0, 0),
                Normal    = new( 0, 0,-1),
                UVMin     = new( 0, 0),
                UVMax     = new( 1, 1),
                FaceCoord = 4,
            },
            // Z+
            new MeshQuad()
            {
                VertA     = new( 1, 0, 1),
                VertB     = new( 1, 1, 1),
                VertC     = new( 0, 1, 1),
                VertD     = new( 0, 0, 1),
                Normal    = new( 0, 0, 1),
                UVMin     = new( 0, 0),
                UVMax     = new( 1, 1),
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
