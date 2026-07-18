using System;
using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.Events;

namespace GyeNyame.Combat.Components
{
    public class EntityHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] protected float maxHealth = 100f;
        
        public event Action<DamageData> OnDamageTakenEvent;

        protected float currentHealth;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(DamageData damageData)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damageData.Amount;

            EventBus.Publish(new EntityDamagedMessage(transform.root.gameObject, damageData));

            OnDamageTakenEvent?.Invoke(damageData);
            OnDamageReceived(damageData);

            CheckDeath();
        }

        private void CheckDeath()
        {
            if (currentHealth > 0) return;
            Die();
        }

        protected virtual void OnDamageReceived(DamageData data)
        {
            Debug.Log($"{gameObject.name} received {data.Amount} damage. Health: {currentHealth}/{maxHealth}");
        }

        protected virtual void Die()
        {
            Debug.Log($"{gameObject.name} died!");
            gameObject.SetActive(false);
        }
    }
}
