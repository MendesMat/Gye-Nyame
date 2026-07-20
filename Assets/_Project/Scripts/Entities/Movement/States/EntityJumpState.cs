using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Entities.Movement.States
{
    public class EntityJumpState : BaseState
    {
        private readonly IEntityLocomotion _context;
        public override EntityStateCategory StateCategory => EntityStateCategory.Jump;

        public EntityJumpState(IStateMachine stateMachine, IEntityLocomotion context) 
            : base(stateMachine)
        {
            _context = context;
        }

        public override void Enter() => _context.ExecuteJump();

        public override void FixedUpdate()
        {
            _context.UpdateMovement(
                speedMultiplier: _context.AirSpeedMultiplier, 
                lockDepth: _context.LockDepthDuringJump
            );

            _context.ConsumeDashRequest();

            if (_context.VerticalVelocity <= 0f)
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityFallState>());
            }
        }
    }
}
