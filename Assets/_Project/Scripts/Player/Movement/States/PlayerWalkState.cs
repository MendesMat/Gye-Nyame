using GyeNyame.Core.StateMachine;
using GyeNyame.Agents.Movement.States;

namespace GyeNyame.Player.Movement.States
{
    public class PlayerWalkState : EntityWalkState
    {
        private readonly IPlayerMovementContext playerContext;

        public PlayerWalkState(IStateMachine stateMachine, IPlayerMovementContext playerContext) : base(stateMachine, playerContext)
        {
            this.playerContext = playerContext;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (playerContext.ConsumeDashRequest())
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerDashState>());
                return;
            }

            if (playerContext.ConsumeJumpRequest())
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerJumpState>());
            }
        }

        protected override void TransitionToIdle() => StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerIdleState>());
        protected override void TransitionToFall() => StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerFallState>());
    }
}
