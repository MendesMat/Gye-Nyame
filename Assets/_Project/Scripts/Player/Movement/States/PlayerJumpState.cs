using GyeNyame.Core.StateMachine;

namespace GyeNyame.Player.Movement.States
{
    public class PlayerJumpState : BaseState
    {
        private readonly IPlayerMovementContext _context;
        public override EntityStateCategory StateCategory => EntityStateCategory.Jump;

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

            _context.ConsumeDashRequest();

            if (_context.VerticalVelocity <= 0f)
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerFallState>());
            }
        }
    }
}
