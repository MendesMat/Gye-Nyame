using GyeNyame.Core.StateMachine;

namespace GyeNyame.Player.Movement.States
{
    public class PlayerFallState : BaseState
    {
        private readonly IPlayerMovementContext _context;

        public PlayerFallState(IStateMachine stateMachine, IPlayerMovementContext context) : base(stateMachine)
        {
            _context = context;
        }

        public override void FixedUpdate()
        {
            _context.UpdateMovement
            (
                speedMultiplier: _context.AirSpeedMultiplier, 
                lockDepth: _context.LockDepthDuringJump
            );

            _context.ConsumeDashRequest();

            if (!_context.IsGrounded) return;

            if (_context.HasMoveInput)
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerWalkState>());
                return;
            }

            StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerIdleState>());
        }
    }
}
