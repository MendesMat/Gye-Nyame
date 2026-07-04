using GyeNyame.Core.StateMachine;

namespace GyeNyame.Player.Movement.States
{
    public class PlayerIdleState : BaseState
    {
        private readonly IPlayerMovementContext _context;

        public PlayerIdleState(IStateMachine stateMachine, IPlayerMovementContext context) : base(stateMachine)
        {
            _context = context;
        }

        public override void FixedUpdate()
        {
            _context.UpdateMovement(speedMultiplier: 0f, lockDepth: false);

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

            if (_context.HasMoveInput)
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerWalkState>());
            }
        }
    }
}
