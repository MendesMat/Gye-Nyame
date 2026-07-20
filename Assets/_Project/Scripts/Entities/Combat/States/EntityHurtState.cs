using UnityEngine;
using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Data;

namespace GyeNyame.Entities.Combat.States
{
    public class EntityHurtState : BaseState
    {
        protected float hurtDuration;
        protected float timer;
        protected DamageData currentDamage;
        protected Vector3 targetPosition;
        protected readonly IEntityLocomotion locomotionContext;
        protected readonly IEntityHealth healthContext;

        private bool _isComboWaitActive;
        private float _comboWaitTimer;
        private const float ComboWaitDuration = 0.5f;

        public override EntityStateCategory StateCategory => EntityStateCategory.Hurt;

        public EntityHurtState(IStateMachine stateMachine, IEntityLocomotion locomotionContext, IEntityHealth healthContext) 
            : base(stateMachine)
        {
            this.locomotionContext = locomotionContext;
            this.healthContext = healthContext;
        }

        public void InitializeHurt(DamageData damageData, Vector3 position)
        {
            hurtDuration = damageData.HitStopTime;
            currentDamage = damageData;
            targetPosition = position;
        }

        public override void Enter()
        {
            timer = hurtDuration;
            _isComboWaitActive = false;
            locomotionContext?.SetFacingDirectionLock(true);
            
            if (locomotionContext == null) return;

            Vector3 knockbackDirection = (targetPosition - currentDamage.SourcePosition).normalized;
            knockbackDirection.y = 0;
            
            locomotionContext.ApplyExternalForce(knockbackDirection, currentDamage.KnockbackForce, currentDamage.KnockupForce);
        }

        public override void Update()
        {
            timer -= Time.deltaTime;

            if (timer > 0f) return;
            
            if (currentDamage.KnockupForce > 0f && locomotionContext != null && !locomotionContext.IsGrounded) return;

            if (healthContext != null && healthContext.IsDead)
            {
                if (!_isComboWaitActive)
                {
                    _isComboWaitActive = true;
                    _comboWaitTimer = ComboWaitDuration;
                }
                
                _comboWaitTimer -= Time.deltaTime;
                if (_comboWaitTimer > 0f) return;
                
                TransitionToDeath();
                return;
            }
            
            TransitionToIdle();
        }

        protected virtual void TransitionToIdle()
        {
            StateMachine.ChangeState(StateMachine.GetOrCreateState<Movement.States.EntityIdleState>());
        }

        protected virtual void TransitionToDeath()
        {
            StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityDeadState>());
        }

        public override void FixedUpdate() => locomotionContext?.UpdateMovement(0f, false);

        public override void Exit()
        {
            base.Exit();
            locomotionContext?.SetFacingDirectionLock(false);
        }
    }
}
