using FainEngine_v2.Core;
using FainEngine_v2.UI.Styling;
using FainEngine_v2.UI.UIElements.Types;
using FainEngine_v2.Utils;
using System.Drawing;

namespace FainCraft.UI;

internal class FPSPanel : UIElement
{
    readonly Label _label;
    readonly IGameTime _gameTime;

    public FPSPanel()
    {
        _gameTime = DependencyInjector.Resolve<IGameTime>();

        _label = AddChild(new UIElement()
        {
            Styles = new ElementStyles()
            {
                BackgroundColour = Color.Beige,
            }
        }).AddChild(new Label()
        {
            Text = "Test String To Be Rendered",
            Styles = new ElementStyles()
            {
                FontSize = 20,
                TextColour = Color.Black
            }
        });
    }

    private float _lastUpdate = 0f;
    public void Update()
    {

        if (_gameTime.TotalTime - _lastUpdate > 0.25f)
        {
            _label.Text = $"{1f / _gameTime.DeltaTime:0} FPS";
            _lastUpdate = _gameTime.TotalTime;
        }
    }
}
