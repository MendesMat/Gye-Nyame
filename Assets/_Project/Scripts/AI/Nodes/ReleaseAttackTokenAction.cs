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
        name: "ReleaseAttackToken", 
        story: "Agent [Agent] releases token to [Director]", 
        category: "Action", 
        id: "release_token_1")]
    public partial class ReleaseAttackTokenAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Director;

        protected override Status OnStart()
        {
            if (Agent.Value == null) return Status.Failure;
            if (Director.Value == null) return Status.Failure;

            var director = Director.Value.GetComponent<IAttackDirector>();
            if (director == null) return Status.Failure;

            int enemyId = Agent.Value.GetInstanceID();
            director.ReleaseAttackToken(enemyId);

            EventBus.Publish(new AINodeStateMessage(Agent.Value, "ReleaseAttackToken", "Sucesso - Token Liberado"));
            return Status.Success;
        }

        protected override Status OnUpdate() => Status.Success;
    }
}
