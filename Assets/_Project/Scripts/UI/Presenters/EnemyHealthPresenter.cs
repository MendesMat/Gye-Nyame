using UnityEngine;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Attributes;
using GyeNyame.UI.Views;

namespace GyeNyame.UI.Presenters
{
    public class EnemyHealthPresenter : MonoBehaviour
    {
        [SerializeField, TagSelector] private string targetTag;
        [SerializeField] private UIHealthBar healthBar;

        private GameObject currentEnemy;

        private void Start()
        {
            gameObject.SetActive(false);
            
            EventBus.Subscribe<EntityDamagedMessage>(OnEntityDamaged);
            EventBus.Subscribe<EntityDeadMessage>(OnEntityDead);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<EntityDamagedMessage>(OnEntityDamaged);
            EventBus.Unsubscribe<EntityDeadMessage>(OnEntityDead);
        }

        private void OnEntityDamaged(EntityDamagedMessage message)
        {
            if (message.Target == null) return;
            if (!message.Target.CompareTag(targetTag)) return;

            currentEnemy = message.Target;
            gameObject.SetActive(true);

            IEntityHealth health = currentEnemy.GetComponentInChildren<IEntityHealth>();
            UpdateHealthBar(health);
        }

        private void OnEntityDead(EntityDeadMessage message)
        {
            if (message.Target == null) return;
            if (currentEnemy == null) return;
            if (message.Target != currentEnemy) return;

            gameObject.SetActive(false);
            currentEnemy = null;
        }

        private void UpdateHealthBar(IEntityHealth health)
        {
            if (health == null) return;
            if (healthBar == null) return;
            
            if (health.MaxHealth <= 0) return;

            float normalizedHealth = health.CurrentHealth / health.MaxHealth;
            healthBar.UpdateFill(normalizedHealth);
        }
    }
}
