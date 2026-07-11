using FainEngine_v2.Core;
using FainEngine_v2.UI.Rendering;
using FainEngine_v2.UI.Styling.Fonts.AtlasFont;

namespace FainCraft.Scenes;
internal class UITestScene : IScene
{
    EntityManager _entityManager = new();

    public void OnLoad()
    {
        _entityManager.Spawn(new UICanvasRenderer());
        //FontAtlasBuilder.RequestFont(null, 32, out var atlas);
    }

    public void Update()
    {
        _entityManager.Update();
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
