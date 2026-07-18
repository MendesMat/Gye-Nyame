using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Combat.Data;

namespace GyeNyame.Combat.Components
{
    [RequireComponent(typeof(Collider))]
    public class HitboxComponent : MonoBehaviour, IHitbox
    {
        [SerializeField] private Collider hitboxCollider;
        [SerializeField] private AttackDataSO attackData;

        public void EnableHitbox()
        {
            if (attackData == null)
            {
                Debug.LogWarning("EnableHitbox called without a bound AttackDataSO!");
            }

            hitboxCollider.enabled = true;
        }

        public void DisableHitbox()
        {
            hitboxCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (attackData == null) return;

            if (other.TryGetComponent(out IHurtbox hurtbox))
            {
                DamageData damagePayload = new DamageData
                (
                    attackData.Damage,
                    transform.root.position,
                    attackData.KnockbackForce,
                    attackData.KnockupForce,
                    attackData.HitStopTime
                );

                hurtbox.ReceiveHit(damagePayload);
            }
        }
    }
}
