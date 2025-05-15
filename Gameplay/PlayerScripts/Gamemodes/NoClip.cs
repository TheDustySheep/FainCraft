using FainCraft.Gameplay.Motors;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts.Gamemodes
{
    public class NoClip : IState<Vector2>
    {
        readonly EntityMotor motor;
        readonly Transform camTransform;
        float moveSpeed = 20f;
        float sprintSpeed = 100f;

        public NoClip(EntityMotor motor, Transform camTransform)
        {
            this.motor = motor;
            this.camTransform = camTransform;
        }

        public void OnEnter()
        {
            motor.EnableGravity = false;
            motor.EnableCollision = false;
        }

        public IState<Vector2> Tick(Vector2 moveInputs)
        {
            float speed = GameInputs.IsKeyHeld(Key.ControlLeft) ? sprintSpeed : moveSpeed;

            Vector3 velocity = default;
            velocity += camTransform.Forward * moveInputs.Y    * speed;
            velocity += camTransform.Right   * moveInputs.X    * speed;
            velocity += Vector3.UnitY        * VerticalInput() * speed;

            motor.Velocity = velocity;

            return this;
        }

        public void OnExit()
        {

        }

        private float VerticalInput()
        {
            float input = 0f;
            if (GameInputs.IsKeyHeld(Key.Space))
                input += 1f;
            if (GameInputs.IsKeyHeld(Key.ShiftLeft))
                input -= 1f;
            return input;
        }
    }
}
