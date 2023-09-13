using FainCraft.Gameplay.Motors;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Extensions;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts;
internal class PlayerCharacterController
{
    readonly Transform camTransform;
    readonly EntityMotor motor;

    bool _isSprinting;
    readonly float jumpPeriod = 0.5f;
    float jumpCooldown = 0f;
    readonly float moveSpeed = 4.3f;
    readonly float sprintSpeed = 7.1f;
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
        Vector3 velocity = motor.Velocity;

        // Movement
        Vector3 movements = Movement();
        velocity.X = movements.X;
        velocity.Z = movements.Z;

        if (GameInputs.IsKeyHeld(Key.ControlLeft))
            _isSprinting = true;

        // Jumping
        if (GameInputs.IsKeyHeld(Key.Space) && motor.groundedState.IsGrounded && jumpCooldown == 0f)
        {
            float force = MathF.Sqrt(2 * motor.Gravity * 1.25f);
            velocity.Y = force;
            jumpCooldown = jumpPeriod;
        }
        jumpCooldown = MathUtils.Max(0f, jumpCooldown - GameTime.DeltaTime);

        // Flying
        if (GameInputs.IsKeyHeld(Key.F))
        {
            velocity = camTransform.Forward * 100f;
        }

        motor.Velocity = velocity;

        if (GameInputs.IsKeyDown(Key.Escape))
            GameInputs.ExitProgram();
    }

    private Vector3 Movement()
    {
        Vector2 inputs = MovementInputs();

        Vector2 forward = camTransform.Forward.ToXZ().Normalized();
        Vector2 right = camTransform.Right.ToXZ().Normalized();

        if (_isSprinting)
        {
            inputs.Y *= sprintSpeed;
            inputs.X *= moveSpeed;
        }
        else
        {
            inputs *= moveSpeed;
        }

        Vector2 moveVector = Vector2.Zero;
        moveVector += inputs.Y * forward;
        moveVector += inputs.X * right;
        return moveVector.ToXZ();
    }

    private Vector2 MovementInputs()
    {
        Vector2 input = Vector2.Zero;

        if (GameInputs.IsKeyHeld(Key.W))
        {
            input.Y += 1f;
        }
        else if (GameInputs.IsKeyHeld(Key.S))
        {
            input.Y -= 1f;
            _isSprinting = false;
        }
        else
        {
            _isSprinting = false;
        }

        if (GameInputs.IsKeyHeld(Key.A))
        {
            input.X -= 1f;
        }
        if (GameInputs.IsKeyHeld(Key.D))
        {
            input.X += 1f;
        }

        return input.Normalized();
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
