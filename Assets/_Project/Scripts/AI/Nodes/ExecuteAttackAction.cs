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
        name: "ExecuteAttack", 
        story: "Agent [Agent] executes attack", 
        category: "Action", 
        id: "executeattack_1")]
    public partial class ExecuteAttackAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        private IEnemyCombat _enemyCombat;

        protected override Status OnStart()
        {
            if (Agent.Value == null) return Status.Failure;
            _enemyCombat = Agent.Value.GetComponent<IEnemyCombat>();
            if (_enemyCombat == null) return Status.Failure;

            _enemyCombat.TryAttack();
            EventBus.Publish(new AINodeStateMessage(Agent.Value, "ExecuteAttack", "Iniciou - Atacando"));
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (_enemyCombat == null) return Status.Failure;
            
            if (!_enemyCombat.IsAttacking)
            {
                EventBus.Publish(new AINodeStateMessage(Agent.Value, "ExecuteAttack", "Sucesso - Ataque Finalizado"));
                return Status.Success;
            }
            
            return Status.Running;
        }
    }
}
