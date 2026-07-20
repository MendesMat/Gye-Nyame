using UnityEngine;
using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Entities.Movement.States
{
    public class EntityDashState : BaseState
    {
        private readonly IEntityLocomotion _context;
        public override EntityStateCategory StateCategory => EntityStateCategory.Dash;

        private Vector2 _dashDirection;
        private float _dashStartTime;

        public EntityDashState(IStateMachine stateMachine, IEntityLocomotion context) : base(stateMachine)
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
                StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityIdleState>());

            StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityWalkState>());
        }
    }
}
