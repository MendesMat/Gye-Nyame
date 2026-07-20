using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Entities.Movement.States
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
            locomotionContext.UpdateMovement(locomotionContext.AirSpeedMultiplier, locomotionContext.LockDepthDuringJump);
            locomotionContext.ConsumeDashRequest();

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
