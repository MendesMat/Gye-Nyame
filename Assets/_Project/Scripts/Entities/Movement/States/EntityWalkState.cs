using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Entities.Movement.States
{
    public class EntityWalkState : BaseState
    {
        protected readonly IEntityLocomotion locomotionContext;
        public override EntityStateCategory StateCategory => EntityStateCategory.Walk;

        public EntityWalkState(IStateMachine stateMachine, IEntityLocomotion locomotionContext) : base(stateMachine)
        {
            this.locomotionContext = locomotionContext;
        }

        public override void FixedUpdate()
        {
            locomotionContext.UpdateMovement(1f, false);

            if (!locomotionContext.IsGrounded)
            {
                TransitionToFall();
                return;
            }

            if (locomotionContext.ConsumeDashRequest())
            {
                TransitionToDash();
                return;
            }

            if (locomotionContext.ConsumeJumpRequest())
            {
                TransitionToJump();
                return;
            }

            if (!locomotionContext.HasMoveInput) TransitionToIdle();
        }

        protected virtual void TransitionToIdle() => StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityIdleState>());
        protected virtual void TransitionToFall() => StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityFallState>());
        protected virtual void TransitionToDash() => StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityDashState>());
        protected virtual void TransitionToJump() => StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityJumpState>());
    }
}
