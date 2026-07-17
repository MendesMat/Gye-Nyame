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

            // Consome a intenção de dash para não enfileirar no ar (sem transição, ignorando a ação)
            _context.ConsumeDashRequest();

            if (_context.VerticalVelocity <= 0f)
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerFallState>());
            }
        }
    }
}
