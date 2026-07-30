using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.StateMachine;
using GyeNyame.Player.Combat.States;
using GyeNyame.Core.Events;
using GyeNyame.Combat.Components;
using GyeNyame.Core.InputBuffer;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Entities.Combat.States;

namespace GyeNyame.Player.Combat
{
    public class PlayerHealth : EntityHealth
    {
        [SerializeField] private StateMachine _stateMachine;

        protected override void Awake()
        {
            base.Awake();
            var locomotion = GetComponent<IEntityLocomotion>();
            var inputBuffer = GetComponentInParent<InputBuffer>();
            var visuals = GetComponentInChildren<IEntityVisuals>();
            
            _stateMachine.RegisterState<PlayerHurtState>(new StateFactory<PlayerHurtState>(sm => new PlayerHurtState(sm, inputBuffer, locomotion, this)));
            _stateMachine.RegisterState<PlayerDeadState>(new StateFactory<PlayerDeadState>(sm => new PlayerDeadState(sm, locomotion, this, visuals)));
        }

        protected override void OnDamageReceived(DamageData data)
        {
            base.OnDamageReceived(data);

            if (_stateMachine.CurrentState is BaseEntityAttackState attackState)
            {
                if (!IsDead) return;

                DeferDeath();
                attackState.OnStateExit += OnAttackStateExited;
                return;
            }

            var hurtState = _stateMachine.GetOrCreateState<PlayerHurtState>();
            hurtState.InitializeHurt(data, transform.position);
            
            _stateMachine.ChangeState(hurtState);
        }

        private void OnEnable()
        {
            if (_stateMachine != null)
                _stateMachine.OnStateChanged += OnCombatStateChanged;
        }

        private void OnDisable()
        {
            if (_stateMachine != null)
                _stateMachine.OnStateChanged -= OnCombatStateChanged;
        }

        private void OnAttackStateExited(BaseState state)
        {
            state.OnStateExit -= OnAttackStateExited;
            ExecuteDeferredDeath();
        }

        private void OnCombatStateChanged(BaseState previous, BaseState next)
        {
            if (!IsDead) return;

            if (next is PlayerDeadState or PlayerHurtState or BaseEntityAttackState) return;

            _stateMachine.ChangeState(_stateMachine.GetOrCreateState<PlayerDeadState>());
        }
    }
}
