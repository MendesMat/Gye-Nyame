using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Agents.Combat.States;
using GyeNyame.Agents.Movement.States;

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
            if (locomotion == null) UnityEngine.Debug.LogError($"[Debugger] O Inimigo {gameObject.name} nao tem o script EnemyMovement (ou outro IEntityLocomotion) atrelado!");

            stateMachine.RegisterState<EntityIdleState>(new StateFactory<EntityIdleState>(sm => new EntityIdleState(sm, locomotion)));
            stateMachine.RegisterState<EntityWalkState>(new StateFactory<EntityWalkState>(sm => new EntityWalkState(sm, locomotion)));
            stateMachine.RegisterState<EntityFallState>(new StateFactory<EntityFallState>(sm => new EntityFallState(sm, locomotion)));
            stateMachine.RegisterState<EntityHurtState>(new StateFactory<EntityHurtState>(sm => new EntityHurtState(sm, locomotion)));
        }

        protected virtual void OnEnable()
        {
            EventBus.Subscribe<EntityDamagedMessage>(OnEntityDamaged);
        }

        protected virtual void OnDisable()
        {
            EventBus.Unsubscribe<EntityDamagedMessage>(OnEntityDamaged);
        }

        private void OnEntityDamaged(EntityDamagedMessage message)
        {
            if (message.Target != gameObject) return;
            ApplyKnockback(message.Damage);
        }

        protected virtual void ApplyKnockback(DamageData data)
        {
            var hurtState = stateMachine.GetOrCreateState<EntityHurtState>();
            hurtState.InitializeHurt(data, transform.root.position);
            stateMachine.ChangeState(hurtState);
        }
    }
}
