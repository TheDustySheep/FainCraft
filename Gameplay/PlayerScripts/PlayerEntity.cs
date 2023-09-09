using FainCraft.Gameplay.Motors;
using FainCraft.Gameplay.WorldScripts.Chunking;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Cameras;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;
internal class PlayerEntity : GameObject
{
    public readonly Transform camTransform = new Transform();
    public readonly Camera3D camera;
    public readonly PlayerCharacterController controller;
    public readonly EntityMotor Motor;
    public readonly WorldEditor worldEditor;

    public PlayerEntity(IWorldData worldData)
    {
        Transform.LocalPosition = new Vector3(32, 44, 32);
        camTransform.LocalPosition = new Vector3(0, 0.9f, 0);
        camTransform.SetParent(Transform);

        camera = new Camera3D(camTransform);
        camera.SetMainCamera();

        Motor = new EntityMotor(Transform, worldData);
        worldEditor = new WorldEditor(camTransform, worldData);
        controller = new PlayerCharacterController(camTransform, Motor);
    }

    public override void Update()
    {
        controller.Update();
        camera.Update();
        worldEditor.Update();
        Motor.Update();
    }

    public override void FixedUpdate()
    {
        Motor.FixedUpdate();
    }
}
