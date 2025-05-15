using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainCraft.Gameplay.PlayerScripts
{
    public class StateMachine<T>
    {
        public IState<T> CurrentState => _state;
        IState<T> _state;

        public StateMachine(IState<T> state)
        {
            _state = state;
            _state.OnEnter();
        }

        public void EnterState(IState<T> state)
        {
            if (state == _state)
                return;

            _state.OnExit();
            _state = state;
            _state.OnEnter();
        }

        public void Tick(T data)
        {
            IState<T> newState = _state.Tick(data);

            if (newState == _state)
                return;

            EnterState(newState);
        }
    }

    public interface IState<T>
    {
        public void OnEnter();
        public IState<T> Tick(T data);
        public void OnExit();
    }
}
