using FainCraft.Gameplay.Motors;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Extensions;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts.MovementState
{
    public class Walking : IState<Vector2>
    {
        private readonly EntityMotor _motor;
        private readonly Transform _camTransform;

        bool _isSprinting;
        float jumpCooldown = 0f;

        readonly float jumpPeriod = 0.5f;
        readonly float moveSpeed = 4.3f;
        readonly float sprintSpeed = 5.6f;

        public Walking(EntityMotor motor, Transform camTransform)
        {
            _motor = motor;
            _camTransform = camTransform;
        }

        public void OnEnter()
        {

        }

        public IState<Vector2> Tick(Vector2 moveInputs)
        {
            Vector3 velocity = _motor.Velocity;

            // Movement
            Vector3 movements = Movement(moveInputs);
            velocity.X = movements.X;
            velocity.Z = movements.Z;

            if (GameInputs.IsKeyHeld(Key.ControlLeft))
                _isSprinting = true;

            // Jumping
            if (GameInputs.IsKeyHeld(Key.Space) && _motor.GroundedState.IsGrounded && jumpCooldown == 0f)
            {
                float force = MathF.Sqrt(2 * _motor.Gravity * 1.25f);
                velocity.Y = force;
                jumpCooldown = jumpPeriod;
            }
            jumpCooldown = MathUtils.Max(0f, jumpCooldown - GameTime.DeltaTime);

            // Flying
            if (GameInputs.IsKeyHeld(Key.F))
            {
                velocity = _camTransform.Forward * 100f;
            }

            _motor.Velocity = velocity;

            return this;
        }

        public void OnExit()
        {

        }

        private Vector3 Movement(Vector2 inputs)
        {
            Vector2 forward = _camTransform.Forward.ToXZ().Normalized();
            Vector2 right   = _camTransform.Right  .ToXZ().Normalized();

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
