using FainEngine_v2.UI;
using FainEngine_v2.UI.Elements;
using System.Drawing;

namespace FainCraft.UI.Panels
{
    public class StartMenuUIPanel : UIElement
    {
        public StartMenuUIPanel(UICanvas canvas)
        {
            Align = Layout.Align.Center;
            Justify = Layout.Justify.Center;

            XSizeMode = Layout.SizeMode.Grow;
            YSizeMode = Layout.SizeMode.Grow;

            AddChildren(
                new UIElement()
                {
                    BackgroundColour = Color.BlanchedAlmond,
                }
                .AddChildren(
                    new UIText(canvas, "Singleplayer"),
                    new UIText(canvas, "Options"),
                    new UIText(canvas, "Exit")
                )
            );
        }
    }
}
