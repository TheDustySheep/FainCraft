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
    public readonly Transform camTransform = new Transform();
    public readonly Camera3D camera;
    public readonly PlayerCharacterController controller;
    public readonly EntityMotor Motor;
    public readonly WorldEditor worldEditor;

    public StaticAABB Bounds => new StaticAABB()
    {
        Position = Motor.PositionStart,
        Size = Motor.PlayerSize,
    };

    public PlayerEntity(World world)
    {
        Transform.LocalPosition = new Vector3(32, 44, 32);
        camTransform.LocalPosition = new Vector3(0, 0.85f, 0);
        camTransform.SetParent(Transform);

        camera = new Camera3D(camTransform);
        camera.SetMainCamera();

        Motor = new EntityMotor(Transform, world.WorldData);
        worldEditor = new WorldEditor(camTransform, world);
        controller = new PlayerCharacterController(camTransform, Motor);

        world.WorldEntityController.RegisterEntity(this);
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
