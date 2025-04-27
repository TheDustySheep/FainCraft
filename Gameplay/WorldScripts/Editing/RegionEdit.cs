using FainCraft.Gameplay.WorldScripts.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.WorldScripts.Editing;

internal struct RegionEdit
{
    public VoxelCoordRegion Coord;
    public IVoxelEdit Edit;
}
