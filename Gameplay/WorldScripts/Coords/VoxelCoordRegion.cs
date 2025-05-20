using static FainCraft.Gameplay.WorldScripts.WorldConstants;

namespace FainCraft.Gameplay.WorldScripts.Coords
{
    public readonly struct VoxelCoordRegion
    {
        public readonly VoxelCoordLocal LocalCoord;
        public readonly int Chunk_Y;

        public VoxelCoordRegion(VoxelCoordGlobal global)
        {
            Chunk_Y = ConvertToChunkFromGlobal(global.Y);
            LocalCoord = (VoxelCoordLocal)global;
        }

        public VoxelCoordRegion(int l_x, int g_y, int l_z)
        {
            Chunk_Y = ConvertToChunkFromGlobal(g_y);
            int l_y = ConvertToLocalFromGlobal(g_y);

            LocalCoord = new VoxelCoordLocal(l_x, l_y, l_z);
        }

        public VoxelCoordRegion(int l_x, int l_y, int l_z, int c_y)
        {
            Chunk_Y = c_y;
            LocalCoord = new VoxelCoordLocal(l_x, l_y, l_z);
        }
    }
}
