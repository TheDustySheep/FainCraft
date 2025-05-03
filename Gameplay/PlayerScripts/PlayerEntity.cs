using FainCraft.Gameplay.Motors;
using FainCraft.Gameplay.WorldEntities;
using FainCraft.Gameplay.WorldScripts;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Entities;
using FainEngine_v2.Physics.AABB;
using FainEngine_v2.Rendering.Cameras;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;
internal class PlayerEntity : GameObject, IWorldEntity
{
    public readonly Transform camTransform = new();
    public readonly Camera3D camera;
    public readonly PlayerCharacterController controller;
    public readonly EntityMotor Motor;
    public readonly WorldEditor worldEditor;

    public StaticAABB Bounds => new StaticAABB()
    {
        Position = Motor.TransformPosition,
        Size = Motor.PlayerSize,
    };

    public PlayerEntity(World world)
    {
        Transform.LocalPosition = new Vector3(32, 130, 32);
        camTransform.LocalPosition = new Vector3(0, 0.85f, 0);
        camTransform.SetParent(Transform);

        camera = new Camera3D(camTransform);
        camera.SetMainCamera();

        Motor = new EntityMotor(new CollisionHandler(world.WorldData), Transform);
        worldEditor = new WorldEditor(camTransform, world);
        controller = new PlayerCharacterController(camTransform, Motor);
        Motor.EnableGravity = false;
        world.WorldEntityController.RegisterEntity(this);

        Task.Run(async () =>
        {
            await Task.Delay(500);
            Motor.EnableGravity = true;
        });
    }

    public override void Update()
    {
        controller.Update();
        camera.Update();
        worldEditor.Update();
        Motor.Update();

        SharedVariables.PlayerPosition.Value = new PlayerPosition(Transform.GlobalPosition);
    }

    public override void FixedUpdate()
    {
        Motor.FixedUpdate();
    }
}
