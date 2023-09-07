using FainCraft.Gameplay.WorldScripts.Chunking;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Cameras;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;
internal class PlayerEntity : GameObject
{
    readonly Transform camTransform = new Transform();
    readonly Camera3D camera;
    readonly PlayerCharacterController controller;
    readonly PlayerMotor Motor;

    public PlayerEntity(IWorldData worldData)
    {
        Transform.Position = new Vector3(0, 35, 0);
        camTransform.Position = new Vector3(0, 1.8f, 0);
        camTransform.SetParent(Transform);

        camera = new Camera3D(camTransform);
        camera.SetMainCamera();

        Motor = new PlayerMotor(Transform, worldData);

        controller = new PlayerCharacterController(camera, camTransform, Motor);
    }

    public override void Update()
    {
        controller.Update();
        camera.Update();
    }

    public override void FixedUpdate()
    {
        Motor.FixedUpdate();
    }
}
