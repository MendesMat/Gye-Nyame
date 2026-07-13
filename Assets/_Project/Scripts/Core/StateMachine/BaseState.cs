using System;

namespace GyeNyame.Core.StateMachine
{
    public abstract class BaseState
    {
        protected IStateMachine StateMachine { get; }
        public event Action<BaseState> OnStateExit;
        
        public virtual string StateName => GetType().Name;

        protected BaseState(IStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit()
        {
            OnStateExit?.Invoke(this);
        }
    }
}
