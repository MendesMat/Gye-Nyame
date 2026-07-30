using System;
using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.Events;
using GyeNyame.Core.StateMachine;

namespace GyeNyame.Combat.Components
{
    public class EntityHealth : MonoBehaviour, IDamageable, IEntityHealth
    {
        [Header("Components")]
        [SerializeField] protected StateMachine stateMachine;

        [Header("Health Settings")]
        [SerializeField] protected float maxHealth = 100f;
        
        [Header("Physics Settings")]
        [SerializeField] protected string deadLayerName = "Dead";

        public event Action<DamageData> OnDamageTakenEvent;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => currentHealth <= 0;

        protected float currentHealth;
        private int _originalLayer;
        private int _deadLayerIndex;
        private bool _isDeathDeferred;
        private bool _hasPendingDeath;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            _originalLayer = gameObject.layer;
            _deadLayerIndex = LayerMask.NameToLayer(deadLayerName);

            if (stateMachine == null)
            {
                stateMachine = GetComponentInParent<StateMachine>();
            }
        }

        public void SetDeadLayer(bool isDeadLayer)
        {
            gameObject.layer = isDeadLayer ? _deadLayerIndex : _originalLayer;
        }

        public void TakeDamage(DamageData damageData)
        {
            bool wasDead = IsDead;
            
            currentHealth -= damageData.Amount;
            if (currentHealth < 0) currentHealth = 0;

            EventBus.Publish(new EntityDamagedMessage(GetEntityRoot(), damageData));

            OnDamageTakenEvent?.Invoke(damageData);
            OnDamageReceived(damageData);

            if (wasDead) return;
            CheckDeath();
        }

        public void DeferDeath()
        {
            _isDeathDeferred = true;
        }

        public void ExecuteDeferredDeath()
        {
            if (!_hasPendingDeath) return;
            
            _hasPendingDeath = false;
            _isDeathDeferred = false;
            Die();
        }

        public void CancelDeathDeferral()
        {
            _isDeathDeferred = false;
            _hasPendingDeath = false;
        }

        private void CheckDeath()
        {
            if (currentHealth > 0) return;
            
            if (_isDeathDeferred)
            {
                _hasPendingDeath = true;
                return;
            }

            Die();
        }

        protected virtual void OnDamageReceived(DamageData data) { }

        protected virtual void Die()
        {
            EventBus.Publish(new EntityDeadMessage(GetEntityRoot()));
        }

        public GameObject GetEntityRoot()
        {
            if (stateMachine != null) return stateMachine.gameObject;
            return gameObject;
        }
    }
}
