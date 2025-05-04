using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Signals.Gameplay.WorldScripts;
using FainEngine_v2.Core;
using FainEngine_v2.UI;
using FainEngine_v2.UI.Elements;
using FainEngine_v2.Utils;
namespace FainCraft.UI
{
    public class DebugStatsUI : UIElement
    {
        readonly WriteOverQueue<TimeSpan> MeshData = new(16384);
        readonly WriteOverQueue<TimeSpan> TerrainTimes = new(1024);

        uint _meshQueue;
        uint _terrainQueue;
        uint _loadedMeshes;

        UIText FPSText;
        UIText MeshText;
        UIText TerrainText;

        Timer _timer;

        public DebugStatsUI(UICanvas canvas)
        {
            DebugGenerationTimeSignals.OnMeshGenerate    +=     MeshData.Add;
            DebugGenerationTimeSignals.OnTerrianGenerate += TerrainTimes.Add;
            DebugGenerationTimeSignals.OnMeshQueueUpdate    += i => _meshQueue = i;
            DebugGenerationTimeSignals.OnLoadedMeshesUpdate += i => _loadedMeshes = i;
            DebugGenerationTimeSignals.OnTerrainQueueUpdate += i => _terrainQueue = i;

            FPSText     = new UIText(canvas, "FPS");
            MeshText    = new UIText(canvas, "Mesh");
            TerrainText = new UIText(canvas, "Terrain");

            AddChildren(
                FPSText,
                MeshText,
                TerrainText
            );

            LayoutAxis = Layout.Axis.Y;

            _timer = new Timer(i => UpdateText(), null, 0, 500);
        }

        ~DebugStatsUI()
        {
            DebugGenerationTimeSignals.OnMeshGenerate    -=     MeshData.Add;
            DebugGenerationTimeSignals.OnTerrianGenerate -= TerrainTimes.Add;

            _timer.Dispose();
        }

        public void UpdateText()
        {
            FPSText.Text = $"FPS: {1f / GameTime.DeltaTime}";

            if (MeshData.Count > 0)
            {
                var values = MeshData.ToArray();
                var avg = values.Average(i => i.TotalMilliseconds);
                MeshText.Text = $"Mesh Gen\n - Queue: {_meshQueue}\n - {avg:F4}ms/chunk\n - {_loadedMeshes} Meshes Loaded";
            }

            if (TerrainTimes.Count > 0)
            {
                var values = TerrainTimes.ToArray();
                var avg = values.Average(i => i.TotalMilliseconds);
                TerrainText.Text = $"Terrain Gen\n - Queue: {_terrainQueue}\n - {avg:F4}ms/region";
            }

        }
    }
}
