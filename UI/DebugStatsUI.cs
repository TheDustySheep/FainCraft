using FainCraft.Gameplay;
using FainCraft.Gameplay.WorldScripts.Core;
using FainCraft.Signals.Gameplay.WorldScripts;
using FainEngine_v2.Core;
using FainEngine_v2.UI;
using FainEngine_v2.UI.Elements;
using FainEngine_v2.Utils;
using Newtonsoft.Json.Linq;
using System.Text;
namespace FainCraft.UI
{
    public class DebugStatsUI : UIElement
    {
        readonly WriteOverQueue<TimeSpan> MeshTimes    = new(16384);
        readonly WriteOverQueue<TimeSpan> TerrainTimes = new(1024);

        UIText Text;

        Timer _timer;

        public DebugStatsUI(UICanvas canvas)
        {
            DebugGenerationTimeSignals.OnMeshGenerate    +=    MeshTimes.Add;
            DebugGenerationTimeSignals.OnTerrianGenerate += TerrainTimes.Add;

            Text = new UIText(canvas, GenerateText());

            AddChildren(Text);

            LayoutAxis = Layout.Axis.Y;

            _timer = new Timer(i => Text.Text = GenerateText(), null, 0, 500);
        }

        ~DebugStatsUI()
        {
            DebugGenerationTimeSignals.OnMeshGenerate    -=    MeshTimes.Add;
            DebugGenerationTimeSignals.OnTerrianGenerate -= TerrainTimes.Add;

            _timer.Dispose();
        }

        private string GenerateText()
        {
            StringBuilder sb = new(" --- Debug Statistics ---");
            sb.AppendLine($"Frames Per Second");
            sb.AppendLine($" - FPS: {1f / GameTime.DeltaTime:F2}");
            sb.AppendLine($" - TPS: {1f / GameTime.FixedDeltaTime:F2}");
            sb.AppendLine($"");
            sb.AppendLine($"Mesh Generation");
            sb.AppendLine($" - Loaded: {DebugVariables.LoadedMeshCount}");
            sb.AppendLine($" - Queue:  {DebugVariables.MeshQueueCount}");
            if (MeshTimes.Count > 0)
            {
                var data = MeshTimes.ToArray();
                sb.AppendLine($" - Gen Time (avg): {data.Average(i => i.TotalMilliseconds):F5} ms/chunk");
                sb.AppendLine($" - Gen Time (max): {data.Max(i => i.TotalMilliseconds):F5} ms/chunk");
            }
            sb.AppendLine($"");
            sb.AppendLine($"Terrain Generation");
            sb.AppendLine($" - Queue: {DebugVariables.TerrainQueueCount}");
            if (TerrainTimes.Count > 0)
            {
                var data = TerrainTimes.ToArray();
                sb.AppendLine($" - Gen Time (avg): {data.Average(i => i.TotalMilliseconds):F5} ms/chunk");
                sb.AppendLine($" - Gen Time (max): {data.Max(i => i.TotalMilliseconds):F5} ms/chunk");
            }
            sb.AppendLine($"");
            sb.AppendLine($"World");
            sb.AppendLine($" - Loaded: {DebugVariables.WorldLoadedRegions} regions");
            sb.AppendLine($" - Loaded: {DebugVariables.WorldLoadedRegions.Value * WorldConstants.REGION_Y_TOTAL_COUNT} chunks");
            return sb.ToString();
        }
    }
}
