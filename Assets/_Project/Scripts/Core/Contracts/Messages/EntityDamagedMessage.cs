using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.Events;

namespace GyeNyame.Core.Contracts.Messages
{
    public readonly struct EntityDamagedMessage : IMessage
    {
        public GameObject Target { get; }
        public DamageData Damage { get; }

        public EntityDamagedMessage(GameObject target, DamageData damage)
        {
            Target = target;
            Damage = damage;
        }
    }
}
