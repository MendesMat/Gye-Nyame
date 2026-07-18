using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Combat.Components
{
    public class HurtboxComponent : MonoBehaviour, IHurtbox
    {
        [Header("Components")]
        [SerializeField] private Collider hurtboxCollider;
        
        private IDamageable _damageableEntity;

        private void Awake()
        {
            _damageableEntity = GetComponent<IDamageable>();
            if (_damageableEntity == null)
            {
                Debug.LogError($"IDamageable not found on {gameObject.name}!", this);
                return;
            }

            if (hurtboxCollider == null)
            {
                Debug.LogWarning($"Hurtbox collider is not assigned on {gameObject.name}!", this);
            }
        }

        public void ReceiveHit(DamageData damageData) => _damageableEntity?.TakeDamage(damageData);
    }
}
