using FainEngine_v2.Utils.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay
{
    public static class DebugVariables
    {
        public static ReferenceVariable<int> MeshQueueCount    = new();
        public static ReferenceVariable<int> LoadedMeshCount   = new();
        public static ReferenceVariable<int> TerrainQueueCount = new();

        public static ReferenceVariable<int> WorldLoadedRegions = new();
    }
}
