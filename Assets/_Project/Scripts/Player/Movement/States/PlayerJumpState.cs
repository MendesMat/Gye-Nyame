using GyeNyame.Core.StateMachine;

namespace GyeNyame.Player.Movement.States
{
    public class PlayerJumpState : BaseState
    {
        private readonly IPlayerMovementContext _context;

        public PlayerJumpState(IStateMachine stateMachine, IPlayerMovementContext context) : base(stateMachine)
        {
            _context = context;
        }

        public override void Enter()
        {
            _context.ExecuteJump();
        }

        public override void FixedUpdate()
        {
            _context.UpdateMovement(
                speedMultiplier: _context.AirSpeedMultiplier, 
                lockDepth: _context.LockDepthDuringJump
            );

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
