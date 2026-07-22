using UnityEngine;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;
using Unity.Behavior;

namespace GyeNyame.AI.Debugging
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class AINodesDebugger : MonoBehaviour
    {
        [SerializeField] private bool showLogs = true;
        
        private void OnEnable()
        {
            EventBus.Subscribe<AINodeStateMessage>(HandleNodeStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<AINodeStateMessage>(HandleNodeStateChanged);
        }

        private void HandleNodeStateChanged(AINodeStateMessage message)
        {
            if (!showLogs) return;
            if (message.Agent != gameObject) return;
            
            Debug.Log($"[BehaviorTree - <b><color=cyan>{gameObject.name}</color></b>] Nó <color=orange>{message.NodeName}</color>: <color=green>{message.Status}</color>.");
        }
    }
}
