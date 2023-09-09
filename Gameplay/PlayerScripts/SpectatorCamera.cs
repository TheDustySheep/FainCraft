using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Cameras;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;
internal class SpectatorCamera : GameObject
{
    Camera3D camera;
    PlayerEntity player;
    Transform camTransform = new();

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
