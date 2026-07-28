using FainCraft.Gameplay.WorldScripts.Coords;

namespace FainCraft.Gameplay.WorldScripts.Systems.RegionStateManagement;

public delegate ValueTask<StateType> TransitionAction(RegionCoord coord, CancellationToken token);
