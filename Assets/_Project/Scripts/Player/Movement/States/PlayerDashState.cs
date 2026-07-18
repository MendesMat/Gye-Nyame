using UnityEngine;
using GyeNyame.Core.StateMachine;

namespace GyeNyame.Player.Movement.States
{
    public class PlayerDashState : BaseState
    {
        private readonly IPlayerMovementContext _context;
        public override EntityStateCategory StateCategory => EntityStateCategory.Dash;

        private Vector2 _dashDirection;
        private float _dashStartTime;

        public PlayerDashState(IStateMachine stateMachine, IPlayerMovementContext context) : base(stateMachine)
        {
            _context = context;
        }

        public override void Enter()
        {
            _context.ExecuteDash();
            _dashStartTime = Time.time;
            _dashDirection = ResolveDashDirection();
        }

        public override void FixedUpdate()
        {
            _context.UpdateDirectionalMovement(_dashDirection, _context.DashSpeedMultiplier);

            if (HasDashExpired()) TransitionToGroundState();
        }

        private Vector2 ResolveDashDirection()
        {
            if (_context.HasMoveInput) return _context.CurrentMoveInput.normalized;
            return new Vector2(_context.FacingDirectionX, 0f);
        }

        private bool HasDashExpired() => Time.time >= _dashStartTime + _context.DashDuration;

        private void TransitionToGroundState()
        {
            if(!_context.HasMoveInput) 
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerIdleState>());

            StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerWalkState>());
        }
    }
}
