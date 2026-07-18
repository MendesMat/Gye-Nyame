using GyeNyame.Core.StateMachine;
using GyeNyame.Agents.Movement.States;

namespace GyeNyame.Player.Movement.States
{
    public class PlayerFallState : EntityFallState
    {
        private readonly IPlayerMovementContext playerContext;

        public PlayerFallState(IStateMachine stateMachine, IPlayerMovementContext playerContext) : base(stateMachine, playerContext)
        {
            this.playerContext = playerContext;
        }

        public override void FixedUpdate()
        {
            playerContext.UpdateMovement(playerContext.AirSpeedMultiplier, playerContext.LockDepthDuringJump);
            playerContext.ConsumeDashRequest();

            if (!playerContext.IsGrounded) return;

            if (playerContext.HasMoveInput)
            {
                TransitionToWalk();
                return;
            }

            TransitionToIdle();
        }

        protected override void TransitionToIdle() => StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerIdleState>());
        protected override void TransitionToWalk() => StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerWalkState>());
    }
}
