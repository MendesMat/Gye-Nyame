using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Entities.Movement.States
{
    public class EntityIdleState : BaseState
    {
        protected readonly IEntityLocomotion locomotionContext;
        public override EntityStateCategory StateCategory => EntityStateCategory.Idle;

        public EntityIdleState(IStateMachine stateMachine, IEntityLocomotion locomotionContext) : base(stateMachine)
        {
            this.locomotionContext = locomotionContext;
        }

        public override void FixedUpdate()
        {
            locomotionContext.UpdateMovement(0f, false);

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

            if (locomotionContext.HasMoveInput) TransitionToWalk();
        }

        protected virtual void TransitionToWalk() => StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityWalkState>());
        protected virtual void TransitionToFall() => StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityFallState>());
        protected virtual void TransitionToDash() => StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityDashState>());
        protected virtual void TransitionToJump() => StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityJumpState>());
    }
}
