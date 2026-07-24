using UnityEngine;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Attributes;
using GyeNyame.UI.Views;

namespace GyeNyame.UI.Presenters
{
    public class PlayerHealthPresenter : MonoBehaviour
    {
        [SerializeField, TagSelector] private string targetTag;
        [SerializeField] private UIHealthBar healthBar;

        private void Start()
        {
            InitializeHealthBar();
            EventBus.Subscribe<EntityDamagedMessage>(OnEntityDamaged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<EntityDamagedMessage>(OnEntityDamaged);
        }

        private void InitializeHealthBar()
        {
            if (healthBar == null) return;

            var player = GameObject.FindGameObjectWithTag(targetTag);
            if (player == null) return;

            IEntityHealth health = player.GetComponentInChildren<IEntityHealth>();
            UpdateHealthBar(health);
        }

        private void OnEntityDamaged(EntityDamagedMessage message)
        {
            if (message.Target == null) return;
            if (!message.Target.CompareTag(targetTag)) return;

            IEntityHealth health = message.Target.GetComponentInChildren<IEntityHealth>();
            UpdateHealthBar(health);
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
