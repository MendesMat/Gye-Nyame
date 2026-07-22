using UnityEngine;
using GyeNyame.Core.Events;

namespace GyeNyame.Core.Contracts.Messages
{
    public struct AINodeStateMessage : IMessage
    {
        public GameObject Agent { get; }
        public string NodeName { get; }
        public string Status { get; }

        public AINodeStateMessage(GameObject agent, string nodeName, string status)
        {
            Agent = agent;
            NodeName = nodeName;
            Status = status;
        }
    }
}
