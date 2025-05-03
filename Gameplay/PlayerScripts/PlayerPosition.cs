using FainCraft.Gameplay.WorldScripts.Core;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts
{
    public struct PlayerPosition
    {
        public Vector3 Position;
        public VoxelCoordGlobal VoxelCoord;
        public RegionCoord RegionCoord;

        public PlayerPosition(Vector3 position)
        {
            Position    = position;
            VoxelCoord  = new VoxelCoordGlobal(position);
            RegionCoord = (RegionCoord)VoxelCoord;
        }
    }
}