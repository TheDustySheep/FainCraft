using FainCraft.Gameplay.WorldScripts.Data.Voxels;

namespace FainCraft.Gameplay.WorldScripts.Data.ChunkDataArrays;

public interface IChunkDataStorage
{
    public bool IsEmpty();
    public VoxelState Get(uint index);
    public void Set(uint index, VoxelState value);
    public void CopyTo(Span<VoxelState> destination);
}
