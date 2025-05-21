using FainEngine_v2.Utils.Variables;

namespace FainCraft.Gameplay
{
    public static class DebugVariables
    {
        public static ReferenceVariable<int> MeshQueueCount       = new();
        public static ReferenceVariable<int> OpaqueMeshCount      = new();
        public static ReferenceVariable<int> TransparentMeshCount = new();
        public static ReferenceVariable<int> TerrainQueueCount    = new();

        public static ReferenceVariable<int> WorldLoadedRegions   = new();
    }
}
