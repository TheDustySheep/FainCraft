using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Editing
{
    internal struct VoxelEditHolder
    {
        public RegionCoord RegionCoord;
        public VoxelCoordGlobal VoxelCoord;
        public IVoxelEdit Edit;
    }
}
