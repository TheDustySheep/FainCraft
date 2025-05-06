using FainCraft.Gameplay.Motors;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Extensions;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts.Gamemodes
{
    public class Survival : IGamemode
    {
        readonly EntityMotor motor;
        readonly Transform camTransform;

        bool _isSprinting;
        float jumpCooldown = 0f;

        readonly float jumpPeriod = 0.5f;
        readonly float moveSpeed = 4.3f;
        readonly float sprintSpeed = 5.6f;

        public Survival(EntityMotor motor, Transform camTransform)
        {
            this.motor = motor;
            this.camTransform = camTransform;
        }

        public void EnterState()
        {
            motor.EnableGravity = true;
            motor.EnableCollision = true;
        }

        public void UpdatePosition(Vector2 moveInputs)
        {
            Vector3 velocity = motor.Velocity;

            // Movement
            Vector3 movements = Movement(moveInputs);
            velocity.X = movements.X;
            velocity.Z = movements.Z;

            if (GameInputs.IsKeyHeld(Key.ControlLeft))
                _isSprinting = true;

            // Jumping
            if (GameInputs.IsKeyHeld(Key.Space) && motor.GroundedState.IsGrounded && jumpCooldown == 0f)
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

        public void ExitState()
        {

        }

        private Vector3 Movement(Vector2 inputs)
        {
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
    }
}
