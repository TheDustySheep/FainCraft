using FainCraft.Gameplay.Motors;
using FainEngine_v2.Core;
using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Extensions;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts.Gamemodes
{
    public class NoClip : IGamemode
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

        public void EnterState()
        {
            motor.EnableGravity = false;
            motor.EnableCollision = false;
        }

        public void UpdatePosition(Vector2 moveInputs)
        {
            float speed = GameInputs.IsKeyHeld(Key.ControlLeft) ? sprintSpeed : moveSpeed;

            Vector3 velocity = default;
            velocity += camTransform.Forward * moveInputs.Y    * speed;
            velocity += camTransform.Right   * moveInputs.X    * speed;
            velocity += Vector3.UnitY        * VerticalInput() * speed;

            motor.Velocity = velocity;
        }

        public void ExitState()
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
