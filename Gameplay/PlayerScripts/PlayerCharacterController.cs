using FainCraft.Gameplay.Motors;
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
    readonly Transform camTransform;
    readonly EntityMotor motor;

    float jumpPeriod = 0.5f;
    float jumpCooldown = 0f;
    float moveSpeed = 10f;
    readonly float lookSensitivity = 0.1f;
    Vector2 CameraRotation;

    public PlayerCharacterController(Transform camTransform, EntityMotor motor)
    {
        this.camTransform = camTransform;
        this.motor = motor;
    }

    public void Update()
    {
        UpdatePosition();
        UpdateRotation();
    }

    private void UpdatePosition()
    {
        Vector3 targetDelta = MovementInputs();

        targetDelta.Y = 0f;
        if (targetDelta != default)
        {
            targetDelta = targetDelta.Normalized();
        }

        Vector3 velocity = motor.Velocity;

        if (GameInputs.IsKeyHeld(Key.Space) && motor.groundedState.IsGrounded && jumpCooldown == 0f)
        {
            float force = MathF.Sqrt(2 * motor.Gravity * 1.25f);
            velocity.Y = force;
            jumpCooldown = jumpPeriod;
        }

        if (GameInputs.IsKeyDown(Key.F))
        {
            velocity.Y = 0f;
            float force = MathF.Sqrt(2 * motor.Gravity * 20f);
            velocity.Y = force;
        }

        velocity.X = targetDelta.X * moveSpeed;
        velocity.Z = targetDelta.Z * moveSpeed;

        motor.Velocity = velocity;
        jumpCooldown = MathUtils.Max(0f, jumpCooldown - GameTime.DeltaTime);

        if (GameInputs.IsKeyDown(Key.Escape))
            GameInputs.ExitProgram();
    }

    private Vector3 MovementInputs()
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

        return targetDelta;
    }

    private void UpdateRotation()
    {
        var mouseDelta = GameInputs.MouseDelta;

        if (mouseDelta != default)
        {
            CameraRotation += mouseDelta * lookSensitivity;

            CameraRotation.Y = Math.Clamp(CameraRotation.Y, -89.0f, 89.0f);
            camTransform.LocalRotation = Quaternion.CreateFromYawPitchRoll
            (
                MathUtils.DegreesToRadians(CameraRotation.X),
                MathUtils.DegreesToRadians(CameraRotation.Y),
                0
            );
        }
    }
}
