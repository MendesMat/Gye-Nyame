using UnityEngine;
using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Data;

namespace GyeNyame.Agents.Combat.States
{
    public class EntityHurtState : BaseState
    {
        protected float hurtDuration;
        protected float timer;
        protected DamageData currentDamage;
        protected Vector3 targetPosition;
        
        protected readonly IEntityLocomotion locomotionContext;

        public EntityHurtState(IStateMachine stateMachine, IEntityLocomotion locomotionContext) 
            : base(stateMachine)
        {
            this.locomotionContext = locomotionContext;
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
            locomotionContext?.SetFacingDirectionLock(true);
            
            if (locomotionContext == null) return;
            Vector3 knockbackDirection = (targetPosition - currentDamage.SourcePosition).normalized;
            knockbackDirection.y = 0;
            locomotionContext.ApplyExternalForce(knockbackDirection, currentDamage.KnockbackForce, currentDamage.KnockupForce);
        }

        public override void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0f) locomotionContext?.ReturnToIdle();
        }

        public override void FixedUpdate() => locomotionContext?.UpdateMovement(0f, false);

        public override void Exit()
        {
            base.Exit();
            locomotionContext?.SetFacingDirectionLock(false);
        }
    }
}
