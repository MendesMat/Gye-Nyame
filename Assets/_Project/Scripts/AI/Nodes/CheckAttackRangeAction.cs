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
        name: "CheckAttackRange",
        story: "Checks if [Agent] is in attack range of Player via [Director]",
        category: "Action",
        id: "checkattackrange_action_1")]
    public partial class CheckAttackRangeAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Director;

        protected override Status OnStart()
        {
            if (Agent.Value == null) return Status.Failure;
            if (Director.Value == null) return Status.Failure;

            var director = Director.Value.GetComponent<IAttackDirector>();
            if (director == null) return Status.Failure;

            if (director.IsInAttackRange(Agent.Value))
            {
                EventBus.Publish(new AINodeStateMessage(Agent.Value, "CheckAttackRange", "Sucesso - Target no Range"));
                return Status.Success;
            }
            
            director.ReleaseAttackToken(Agent.Value.GetInstanceID());
            EventBus.Publish(new AINodeStateMessage(Agent.Value, "CheckAttackRange", "Falhou - Fora de Range. Token Liberado"));
            return Status.Failure;
        }

        protected override Status OnUpdate() => Status.Success;
    }
}
