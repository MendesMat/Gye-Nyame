using System;
using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.Events;

namespace GyeNyame.Combat.Components
{
    public class EntityHealth : MonoBehaviour, IDamageable, IEntityHealth
    {
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

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            _originalLayer = gameObject.layer;
            _deadLayerIndex = LayerMask.NameToLayer(deadLayerName);
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

            EventBus.Publish(new EntityDamagedMessage(transform.root.gameObject, damageData));

            OnDamageTakenEvent?.Invoke(damageData);
            OnDamageReceived(damageData);

            if (wasDead) return;
            CheckDeath();
        }

        private void CheckDeath()
        {
            if (currentHealth > 0) return;
            Die();
        }

        protected virtual void OnDamageReceived(DamageData data) { }

        protected virtual void Die()
        {
            EventBus.Publish(new EntityDeadMessage(gameObject));
        }
    }
}
