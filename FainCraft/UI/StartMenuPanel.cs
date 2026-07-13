using FainCraft.Scenes;
using FainEngine_v2.UI.UIElements.Types;
using System.Drawing;

namespace FainCraft.UI;

internal class StartMenuPanel : UIElement
{
    public StartMenuPanel(Action<IScene> loadScene)
    {
        Styles = new FainEngine_v2.UI.Styling.ElementStyles()
        {
            SizeMode = FainEngine_v2.UI.Styling.Fss.Styling.Styles.SizeMode.Fit,

            Axis = FainEngine_v2.UI.Styling.Fss.Styling.Styles.LayoutAxis.Y,
            ChildGap = 20,
            Padding = 16,
            BackgroundColour = Color.Crimson
        };

        AddSceneLink<UITestScene>(loadScene, "UI Test Scene");
        AddSceneLink<SingleplayerWorld>(loadScene, "Singleplayer");
    }

    private void AddSceneLink<T>(Action<IScene> loadScene, string text) where T : IScene, new()
    {
        var elem = AddChild(new UIElement()
        {
            Styles = new()
            {
                BackgroundColour = Color.Beige,
                XPadding = 10,
                XSizeMode = FainEngine_v2.UI.Styling.Fss.Styling.Styles.SizeMode.Grow
            }
        });
        elem.Events.OnMouseClicked += () => loadScene.Invoke(new T());
        elem.AddChild(new Label()
        {
            Text = text,
        });
    }
}
