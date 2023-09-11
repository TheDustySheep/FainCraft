using FainCraft.Gameplay.WorldScripts.Core;

namespace FainCraft.Gameplay.WorldScripts.Editing;
internal interface IVoxelEdit
{
    public GlobalVoxelCoord GlobalCoord { get; }
    public VoxelEditResult Execute(IVoxelEditable worldData);
}
