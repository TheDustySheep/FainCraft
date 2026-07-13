using FainCraft.Gameplay.Motors;
using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts.Gamemodes
{
    public class NoClip : IState<Vector2>
    {
        readonly EntityMotor _motor;
        readonly Transform _camTransform;
        float moveSpeed = 20f;
        float sprintSpeed = 100f;

        private readonly IGameInputs _gameInputs;

        public NoClip(EntityMotor motor, Transform camTransform)
        {
            _motor = motor;
            _camTransform = camTransform;
            _gameInputs = DependencyInjector.Resolve<IGameInputs>();
        }

        public void OnEnter()
        {
            _motor.EnableGravity = false;
            _motor.EnableCollision = false;
        }

        public IState<Vector2> Tick(Vector2 moveInputs)
        {
            float speed = _gameInputs.IsKeyHeld(Key.ControlLeft) ? sprintSpeed : moveSpeed;

            Vector3 velocity = default;
            velocity += _camTransform.Forward * moveInputs.Y    * speed;
            velocity += _camTransform.Right   * moveInputs.X    * speed;
            velocity += Vector3.UnitY        * VerticalInput() * speed;

            _motor.Velocity = velocity;

            return this;
        }

        public void OnExit()
        {

        }

        private float VerticalInput()
        {
            float input = 0f;
            if (_gameInputs.IsKeyHeld(Key.Space))
                input += 1f;
            if (_gameInputs.IsKeyHeld(Key.ShiftLeft))
                input -= 1f;
            return input;
        }
    }
}
