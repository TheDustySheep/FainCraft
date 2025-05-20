using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.OldWorldScripts.Editing
{
    internal struct VoxelEditHolder
    {
        public RegionCoord RegionCoord;
        public VoxelCoordGlobal VoxelCoord;
        public IVoxelEdit Edit;
    }
}
