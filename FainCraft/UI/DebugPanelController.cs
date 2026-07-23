using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering;
using FainEngine_v2.UI;
using FainEngine_v2.UI.Fss.Parsing.Stylesheets;
using FainEngine_v2.UI.UIElements;
using FainEngine_v2.UI.UIElements.Types;
using FainEngine_v2.Utils;

namespace FainCraft.UI;

internal class DebugPanelController : IEntity
{
    readonly Label _fpsLabel;
    readonly Label _drawCallLabel;
    readonly IGameTime _gameTime;
    readonly UIDocument _document;

    public DebugPanelController(ICanvas canvas)
    {
        _gameTime = DependencyInjector.Resolve<IGameTime>();
        var stylesLoader  = DependencyInjector.Resolve<IStylesheetLoader>();
        var elementLoader = DependencyInjector.Resolve<IUIElementLoader>();

        _document = new UIDocument(
            elementLoader.LoadElement("DebugPanel"),
            stylesLoader.LoadFromFile("Resources/UI/DebugPanel/Styles.fss")
        );

        _fpsLabel      = _document.Root.Query<Label>().FilterID("fps").First();
        _drawCallLabel = _document.Root.Query<Label>().FilterID("draw-calls").First(); 

        canvas.AddDocument(_document);
    }

    private float _lastUpdate = 0f;
    public void Update()
    {
        if (_gameTime.TotalTime - _lastUpdate > 0.25f)
        {
            var drawCalls = RenderDebugVariables.DrawCallDebugData.Value;
            _fpsLabel.Text = $"{1f / _gameTime.DeltaTime:0} FPS";
            _drawCallLabel.Text = $"{drawCalls.TotalDrawCalls} Draw Calls";
            _lastUpdate = _gameTime.TotalTime;
        }
    }
}
