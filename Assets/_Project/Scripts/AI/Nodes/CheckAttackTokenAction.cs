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
        name: "CheckAttackToken", 
        story: "Check if [Agent] can get token from [Director]", 
        category: "Action", 
        id: "33ea87ee0436ddca1af0cf3753e47337")]
    public partial class CheckAttackTokenAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Director;
        
        protected override Status OnStart()
        {
            if (Agent.Value == null) return Status.Failure;
            if (Director.Value == null) return Status.Failure;

            var director = Director.Value.GetComponent<IAttackDirector>();
            if (director == null) return Status.Failure;

            if (director.RequestAttackToken(Agent.Value))
            {
                EventBus.Publish(new AINodeStateMessage(Agent.Value, "CheckAttackToken", "Sucesso - Token Adquirido"));
                return Status.Success;
            }
            
            EventBus.Publish(new AINodeStateMessage(Agent.Value, "CheckAttackToken", "Falhou - Sem Token"));
            return Status.Failure;
        }

        protected override Status OnUpdate() => Status.Success;
    }
}
