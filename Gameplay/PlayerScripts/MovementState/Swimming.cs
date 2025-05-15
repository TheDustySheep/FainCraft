using FainCraft.Gameplay.Motors;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Extensions;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts.MovementState
{
    public class Swimming : IState<Vector2>
    {
        private readonly EntityMotor _motor;
        private readonly Transform _camTransform;

        private float _swimSpeed   = 2f;
        private float _swimUpSpeed = 0.5f;

        public Swimming(EntityMotor motor, Transform camTransform)
        {
            _motor = motor;
            _camTransform = camTransform;
        }

        public void OnEnter()
        {
            _motor.Gravity = 3f;
        }

        public IState<Vector2> Tick(Vector2 moveInputs)
        {
            Vector3 velocity = _motor.Velocity;

            // Movement
            Vector3 movements = Movement(moveInputs);
            velocity.X = movements.X;
            velocity.Z = movements.Z;

            // Jumping
            if (GameInputs.IsKeyHeld(Key.Space))
            {
                float force = MathF.Sqrt(2 * _motor.Gravity * _swimUpSpeed);
                velocity.Y = force;
            }

            _motor.Velocity = velocity;

            return this;
        }

        public void OnExit()
        {
            _motor.Gravity = EntityMotor.DEFAULT_GRAVITY;
        }

        private Vector3 Movement(Vector2 inputs)
        {
            Vector2 forward = _camTransform.Forward.ToXZ().Normalized();
            Vector2 right = _camTransform.Right.ToXZ().Normalized();

            inputs *= _swimSpeed;

            Vector2 moveVector = Vector2.Zero;
            moveVector += inputs.Y * forward;
            moveVector += inputs.X * right;
            return moveVector.ToXZ();
        }
    }
}
