using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.AI.Nodes
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "AcquireAttackToken",
        story: "[Agent] acquires attack token from [Director]",
        category: "Action",
        id: "33ea87ee0436ddca1af0cf3753e47337")]
    public partial class AcquireAttackTokenAction : Action
    {
        private const string EventTokenAcquired = "Token acquired";
        private const string EventTokenDenied = "Token denied";

        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Director;

        protected override Status OnStart()
        {
            if (Agent.Value == null) return Status.Failure;
            if (Director.Value == null) return Status.Failure;

            var director = Director.Value.GetComponent<IAttackDirector>();
            if (director == null) return Status.Failure;

            if (!director.RequestAttackToken(Agent.Value))
            {
                PublishState(EventTokenDenied);
                return Status.Failure;
            }

            PublishState(EventTokenAcquired);
            return Status.Success;
        }

        protected override Status OnUpdate() => Status.Success;

        private void PublishState(string message)
        {
            EventBus.Publish(new AINodeStateMessage(Agent.Value, nameof(AcquireAttackTokenAction), message));
        }
    }
}
