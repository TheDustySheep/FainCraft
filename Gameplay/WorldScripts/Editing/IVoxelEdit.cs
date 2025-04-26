using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Editing;
internal interface IVoxelEdit
{
    public VoxelCoordGlobal GlobalCoord { get; }
    public VoxelEditResult Execute(IVoxelEditable worldData);
}
