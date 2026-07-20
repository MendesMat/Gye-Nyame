using UnityEngine;
using GyeNyame.Core.Events;

namespace GyeNyame.Core.Contracts.Messages
{
    public struct EntityDeadMessage : IMessage
    {
        public GameObject Target { get; }

        public EntityDeadMessage(GameObject target)
        {
            Target = target;
        }
    }
}
