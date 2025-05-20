using FainCraft.Gameplay.Motors;
using FainCraft.Gameplay.WorldEntities;
using FainCraft.Gameplay.OldWorldScripts;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Entities;
using FainEngine_v2.Physics.AABB;
using FainEngine_v2.Rendering.Cameras;
using System.Numerics;
using FainCraft.Gameplay.WorldScripts;

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

    public PlayerEntity(Level level)
    {
        Transform.LocalPosition = new Vector3(-300, 130, 100);
        camTransform.LocalPosition = new Vector3(0, 0.85f, 0);
        camTransform.SetParent(Transform);

        camera = new Camera3D(camTransform);
        camera.SetMainCamera();

        Motor = new EntityMotor(new CollisionHandler(level.Provider), Transform);
        worldEditor = new WorldEditor(camTransform, level.Provider);
        controller = new PlayerCharacterController(camTransform, Motor);
        Motor.EnableGravity = false;
        level.Provider.Get<IWorldEntityController>().RegisterEntity(this);

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
