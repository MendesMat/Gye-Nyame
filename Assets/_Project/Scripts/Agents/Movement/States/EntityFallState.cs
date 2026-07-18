using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Agents.Movement.States
{
    public class EntityFallState : BaseState
    {
        protected readonly IEntityLocomotion locomotionContext;
        public override EntityStateCategory StateCategory => EntityStateCategory.Fall;

        public EntityFallState(IStateMachine stateMachine, IEntityLocomotion locomotionContext) : base(stateMachine)
        {
            this.locomotionContext = locomotionContext;
        }

        public override void FixedUpdate()
        {
            locomotionContext.UpdateMovement(0.5f, true);

            if (!locomotionContext.IsGrounded) return;

            if (locomotionContext.HasMoveInput)
            {
                TransitionToWalk();
                return;
            }

            TransitionToIdle();
        }

        protected virtual void TransitionToIdle() => StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityIdleState>());
        protected virtual void TransitionToWalk() => StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityWalkState>());
    }
}
