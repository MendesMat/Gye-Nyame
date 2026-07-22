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
        name: "ProbabilisticTokenRelease", 
        story: "Agent [Agent] probabilistically releases token to [Director]", 
        category: "Action", 
        id: "probabilistic_release_token_1")]
    public partial class ProbabilisticTokenReleaseAction : Action
    {
        private const int MaxProbabilityRange = 20;
        private const int SuccessValue = 0;
        private const string EventStateReleased = "Sucesso - Token Liberado";
        private const string EventStateRetained = "Sucesso - Token Retido";

        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Director;

        protected override Status OnStart()
        {
            if (IsInvalidConfiguration()) return Status.Failure;

            IAttackDirector director = Director.Value.GetComponent<IAttackDirector>();
            if (director == null) return Status.Failure;

            TryReleaseToken(director);

            return Status.Success;
        }

        protected override Status OnUpdate() => Status.Success;

        private void TryReleaseToken(IAttackDirector director)
        {
            int randomValue = UnityEngine.Random.Range(SuccessValue, MaxProbabilityRange);
            if (randomValue == SuccessValue)
            {
                ReleaseToken(director);
                return;
            }

            PublishState(EventStateRetained);
        }

        private void ReleaseToken(IAttackDirector director)
        {
            int enemyId = Agent.Value.GetInstanceID();
            director.ReleaseAttackToken(enemyId);
            PublishState(EventStateReleased);
        }

        private bool IsInvalidConfiguration()
        {
            if (Agent.Value == null) return true;
            if (Director.Value == null) return true;
            
            return false;
        }

        private void PublishState(string message)
        {
            EventBus.Publish(new AINodeStateMessage(Agent.Value, nameof(ProbabilisticTokenReleaseAction), message));
        }
    }
}
