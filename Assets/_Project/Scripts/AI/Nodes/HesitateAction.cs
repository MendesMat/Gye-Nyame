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
        name: "Hesitate",
        story: "Agent [Agent] hesitates before acting",
        category: "Action",
        id: "hesitate_action_1")]
    public partial class HesitateAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<float> MinHesitationTime = new(0.4f);
        [SerializeReference] public BlackboardVariable<float> MaxHesitationTime = new(1.2f);

        private IEnemyMovement _enemyMovement;
        private float _waitTime;
        private float _timerStart;

        protected override Status OnStart()
        {
            if (Agent.Value == null) return Status.Failure;

            _enemyMovement = Agent.Value.GetComponent<IEnemyMovement>();
            if (_enemyMovement == null) return Status.Failure;

            _waitTime = UnityEngine.Random.Range(MinHesitationTime.Value, MaxHesitationTime.Value);
            _timerStart = Time.time;

            _enemyMovement.SetMovementIntent(Vector2.zero);

            EventBus.Publish(new AINodeStateMessage(Agent.Value, "Hesitate", "Pensando..."));
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _enemyMovement.SetMovementIntent(Vector2.zero);

            if (Time.time - _timerStart < _waitTime) return Status.Running;

            EventBus.Publish(new AINodeStateMessage(Agent.Value, "Hesitate", "Sucesso - Hesitação Concluída"));
            return Status.Success;
        }

        protected override void OnEnd()
        {
            if (_enemyMovement == null) return;
            _enemyMovement.SetMovementIntent(Vector2.zero);
        }
    }
}
