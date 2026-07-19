using GyeNyame.Core.StateMachine;
using GyeNyame.Agents.Movement.States;

namespace GyeNyame.Player.Movement.States
{
    public class PlayerIdleState : EntityIdleState
    {
        private readonly IPlayerMovementContext playerContext;

        public PlayerIdleState(IStateMachine stateMachine, IPlayerMovementContext playerContext) : base(stateMachine, playerContext)
        {
            this.playerContext = playerContext;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (StateMachine.CurrentState != this) return;

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

        protected override void TransitionToWalk() => StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerWalkState>());
        protected override void TransitionToFall() => StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerFallState>());
    }
}
