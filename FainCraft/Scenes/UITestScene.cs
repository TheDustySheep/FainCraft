using FainEngine_v2.Core;
using FainEngine_v2.Rendering.PostProcessing;
using FainEngine_v2.Rendering.Sprites;
using FainEngine_v2.Resources;
using FainEngine_v2.UI.Fonts.AtlasFont;
using FainEngine_v2.UI.UI.Drawing;
using System.Numerics;

namespace FainCraft.Scenes;
internal class UITestScene : IScene
{
    EntityManager _entityManager = new();
    TextRenderer _textRenderer = null!;

    public void OnLoad()
    {
        _textRenderer = new TextRenderer();
        _textRenderer.AddCharacter('C');
    }

    public void Update()
    {
        _entityManager.Update();
        _textRenderer.Draw();
    }

    public void FixedUpdate()
    {
        _entityManager.FixedUpdate();
    }

    public void OnUnload()
    {
        _entityManager.DespawnAll();
    }
}
