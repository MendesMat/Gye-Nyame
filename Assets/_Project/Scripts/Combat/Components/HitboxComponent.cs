using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Combat.Data;
using GyeNyame.Core.StateMachine;

namespace GyeNyame.Combat.Components
{
    [RequireComponent(typeof(Collider))]
    public class HitboxComponent : MonoBehaviour, IHitbox
    {
        [Header("Components")]
        [SerializeField] private Collider hitboxCollider;
        [SerializeField] private AttackDataSO attackData;
        [SerializeField] private StateMachine stateMachine;

        public AttackDataSO BoundAttackData => attackData;

        private void Awake()
        {
            if (stateMachine == null)
            {
                stateMachine = GetComponentInParent<StateMachine>();
            }
        }

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
                var originPos = stateMachine != null ? stateMachine.transform.position : transform.position;

                var damagePayload = new DamageData
                (
                    attackData.Damage,
                    originPos,
                    attackData.KnockbackForce,
                    attackData.KnockupForce,
                    attackData.HitStopTime
                );

                hurtbox.ReceiveHit(damagePayload);
            }
        }
    }
}
