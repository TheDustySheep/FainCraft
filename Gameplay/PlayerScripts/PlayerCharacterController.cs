using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Extensions;
using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;
internal class PlayerCharacterController
{
    readonly Camera3D camera;
    readonly Transform camTransform;
    readonly PlayerMotor motor;

    private float moveSpeed = 10f;
    private readonly float lookSensitivity = 0.1f;
    private Vector2 CameraRotation;

    public PlayerCharacterController(Camera3D camera, Transform camTransform, PlayerMotor motor)
    {
        this.camera = camera;
        this.camTransform = camTransform;
        this.motor = motor;
    }

    public void Update()
    {
        UpdatePosition();
        UpdateRotation();
        UpdateZoom();
        UpdateMatrix();
    }

    private void UpdateMatrix()
    {
    }

    private void UpdatePosition()
    {
        Vector3 targetDelta = Vector3.Zero;

        var cameraForward = camTransform.Forward;
        var cameraRight = camTransform.Right;

        if (GameInputs.IsKeyHeld(Key.W))
        {
            //Move forwards
            targetDelta += cameraForward;
        }
        if (GameInputs.IsKeyHeld(Key.S))
        {
            //Move backwards
            targetDelta -= cameraForward;
        }
        if (GameInputs.IsKeyHeld(Key.A))
        {
            //Move left
            targetDelta -= Vector3.Normalize(cameraRight);
        }
        if (GameInputs.IsKeyHeld(Key.D))
        {
            //Move right
            targetDelta += Vector3.Normalize(cameraRight);
        }

        targetDelta.Y = 0f;
        if (targetDelta != default)
        {
            targetDelta = targetDelta.Normalized();
        }


        Vector3 targetVelocity = motor.Velocity;

        if (GameInputs.IsKeyHeld(Key.Space))
        {
            targetVelocity.Y = 1f;
            // v = sqrt(2as) = 2 * gravity * 1 block
            //float force = MathF.Sqrt(2 * 30 * 1.2f);
            //motor.AddVelocity(Vector3.UnitY * force);
        }
        else if (GameInputs.IsKeyHeld(Key.ControlLeft))
        {
            targetVelocity.Y = -1f;
            // v = sqrt(2as) = 2 * gravity * 1 block
            //float force = MathF.Sqrt(2 * 30 * 1.2f);
            //motor.AddVelocity(Vector3.UnitY * force);
        }
        else
        {
            targetVelocity.Y = 0f;
        }

        targetVelocity.X = targetDelta.X * moveSpeed;
        targetVelocity.Z = targetDelta.Z * moveSpeed;
        motor.Velocity = targetVelocity;

        if (GameInputs.IsKeyDown(Key.Escape))
            GameInputs.ExitProgram();
    }

    private void UpdateRotation()
    {
        var mouseDelta = GameInputs.MouseDelta;

        if (mouseDelta != default)
        {
            CameraRotation += mouseDelta * lookSensitivity;

            CameraRotation.Y = Math.Clamp(CameraRotation.Y, -89.0f, 89.0f);
            camTransform.Rotation = Quaternion.CreateFromYawPitchRoll
            (
                MathUtils.DegreesToRadians(CameraRotation.X),
                MathUtils.DegreesToRadians(CameraRotation.Y),
                0
            );
        }
    }

    private void UpdateZoom()
    {
        //We don't want to be able to zoom in too close or too far away so clamp to these values
        moveSpeed = Math.Clamp(moveSpeed - GameInputs.ScrollDelta.Y * 0.1f, 1.0f, 20f);
    }
}
