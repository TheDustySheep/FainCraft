using FainCraft.Gameplay.Motors;
using FainCraft.Gameplay.PlayerScripts.MovementState;
using FainEngine_v2.Core.GameObjects;
using System.Numerics;

namespace FainCraft.Gameplay.PlayerScripts.Gamemodes
{
    public class Survival : IState<Vector2>
    {
        private readonly EntityMotor _motor;
        private readonly Transform   _camTransform;
        private readonly StateMachine<Vector2> _movementStateMachine;

        private readonly Swimming _swimming;
        private readonly Walking  _walking;

        public Survival(EntityMotor motor, Transform camTransform)
        {
            _motor = motor;
            _camTransform = camTransform;

            _swimming = new Swimming(_motor, _camTransform);
            _walking  = new Walking (_motor, _camTransform);

            _movementStateMachine = new StateMachine<Vector2>(
                _walking
            );
        }

        public void OnEnter()
        {
            _motor.EnableGravity = true;
            _motor.EnableCollision = true;
        }

        public IState<Vector2> Tick(Vector2 moveInputs)
        {
            if (_motor.IsOverlapping(i => i.Is_Fluid))
            {
                _movementStateMachine.EnterState(_swimming);
            }
            else
            {
                _movementStateMachine.EnterState(_walking);
            }

            _movementStateMachine.Tick(moveInputs);
            return this;
        }

        public void OnExit()
        {

        }
    }
}
