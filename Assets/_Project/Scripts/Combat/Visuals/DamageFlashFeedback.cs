using System.Collections;
using UnityEngine;
using GyeNyame.Combat.Components;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Combat.Visuals
{
    public class DamageFlashFeedback : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("Settings")]
        [SerializeField] private Material flashMaterial;

        private Material originalMaterial;
        private Coroutine flashCoroutine;
        private EntityHealth _entityHealth;

        private void Awake()
        {
            if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            originalMaterial = spriteRenderer.material;
            _entityHealth = GetComponentInParent<EntityHealth>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EntityDamagedMessage>(OnEntityDamaged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EntityDamagedMessage>(OnEntityDamaged);
        }

        private void OnEntityDamaged(EntityDamagedMessage message)
        {
            var targetObj = _entityHealth != null ? _entityHealth.GetEntityRoot() : transform.root.gameObject;
            if (message.Target != targetObj) return;
            
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRoutine(message.Damage.HitStopTime));
        }

        private IEnumerator FlashRoutine(float hitStopTime)
        {
            ApplyFlashMaterial();
            yield return new WaitForSecondsRealtime(hitStopTime);
            RestoreOriginalMaterial();
        }

        private void ApplyFlashMaterial()
        {
            if (flashMaterial == null) return;
            spriteRenderer.sharedMaterial = flashMaterial;
        }

        private void RestoreOriginalMaterial()
        {
            spriteRenderer.sharedMaterial = originalMaterial;
        }
    }
}
