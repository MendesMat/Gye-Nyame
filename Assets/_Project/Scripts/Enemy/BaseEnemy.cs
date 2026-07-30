using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Entities.Combat.States;
using GyeNyame.Entities.Movement.States;

namespace GyeNyame.Enemy
{
    [RequireComponent(typeof(StateMachine))]
    public abstract class BaseEnemy : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected StateMachine stateMachine;

        protected virtual void Awake()
        {
            var locomotion = GetComponent<IEntityLocomotion>();
            var health = GetComponentInChildren<IEntityHealth>();
            var visuals = GetComponentInChildren<IEntityVisuals>();
            if (locomotion == null) UnityEngine.Debug.LogError($"[Debugger] O Inimigo {gameObject.name} nao tem o script EnemyMovement (ou interface IEntityLocomotion) atrelado no root!");
            if (health == null) UnityEngine.Debug.LogError($"[Debugger] O Inimigo {gameObject.name} nao tem o script EntityHealth (ou interface IEntityHealth) atrelado no root ou filhos!");

            stateMachine.RegisterState<EntityIdleState>(new StateFactory<EntityIdleState>(sm => new EntityIdleState(sm, locomotion)));
            stateMachine.RegisterState<EntityWalkState>(new StateFactory<EntityWalkState>(sm => new EntityWalkState(sm, locomotion)));
            stateMachine.RegisterState<EntityFallState>(new StateFactory<EntityFallState>(sm => new EntityFallState(sm, locomotion)));
            stateMachine.RegisterState<EntityHurtState>(new StateFactory<EntityHurtState>(sm => new EntityHurtState(sm, locomotion, health)));
            stateMachine.RegisterState<EntityDeadState>(new StateFactory<EntityDeadState>(sm => new EntityDeadState(sm, locomotion, health, visuals)));
        }

        protected virtual void OnEnable()
        {
            EventBus.Subscribe<EntityDamagedMessage>(OnEntityDamaged);
            EventBus.Subscribe<PlayerDiedMessage>(OnPlayerDied);
        }

        protected virtual void OnDisable()
        {
            EventBus.Unsubscribe<EntityDamagedMessage>(OnEntityDamaged);
            EventBus.Unsubscribe<PlayerDiedMessage>(OnPlayerDied);
        }

        private void OnEntityDamaged(EntityDamagedMessage message)
        {
            if (message.Target != gameObject) return;
            ApplyKnockback(message.Damage);
        }

        protected virtual void ApplyKnockback(DamageData data)
        {
            if (stateMachine.CurrentState is EntityDeadState) return;
            
            var hurtState = stateMachine.GetOrCreateState<EntityHurtState>();
            hurtState.InitializeHurt(data, transform.position);
            stateMachine.ChangeState(hurtState);
        }

        private void OnPlayerDied(PlayerDiedMessage message)
        {
            stateMachine.ChangeState(stateMachine.GetOrCreateState<EntityIdleState>());
        }
    }
}
