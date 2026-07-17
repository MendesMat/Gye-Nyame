using GyeNyame.Core.StateMachine;

namespace GyeNyame.Player.Movement.States
{
    public class PlayerWalkState : BaseState
    {
        private readonly IPlayerMovementContext _context;

        public PlayerWalkState(IStateMachine stateMachine, IPlayerMovementContext context) : base(stateMachine)
        {
            _context = context;
        }

        public override void FixedUpdate()
        {
            _context.UpdateMovement(speedMultiplier: 1f, lockDepth: false);

            if (!_context.IsGrounded)
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerFallState>());
                return;
            }

            if (_context.ConsumeDashRequest())
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerDashState>());
                return;
            }

            if (_context.ConsumeJumpRequest())
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerJumpState>());
                return;
            }

            if (!_context.HasMoveInput)
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerIdleState>());
            }
        }
    }
}
