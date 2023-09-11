using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Cameras;

namespace FainCraft.Gameplay.PlayerScripts;
internal class SpectatorCamera : GameObject
{
    readonly Camera3D camera;
    readonly PlayerEntity player;
    readonly Transform camTransform = new();

    public SpectatorCamera(PlayerEntity player)
    {
        this.player = player;
        camTransform = player.camTransform;
        camTransform.SetParent(player.Transform);

        camera = new Camera3D(camTransform);
        camera.SetMainCamera();
    }

    public override void Update()
    {

    }
}
