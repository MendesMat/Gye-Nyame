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
        [SerializeReference] public BlackboardVariable<GameObject> Director;
        [SerializeReference] public BlackboardVariable<float> MinHesitationTime = new(0.4f);
        [SerializeReference] public BlackboardVariable<float> MaxHesitationTime = new(1.2f);

        private IEnemyMovement _enemyMovement;
        private IEntityLocomotion _entityLocomotion;
        private IAttackDirector _director;
        private float _waitTime;
        private float _timerStart;

        protected override Status OnStart()
        {
            if (Agent.Value == null) return Status.Failure;

            _enemyMovement = Agent.Value.GetComponent<IEnemyMovement>();
            if (_enemyMovement == null) return Status.Failure;

            _entityLocomotion = Agent.Value.GetComponent<IEntityLocomotion>();

            if (Director != null && Director.Value != null)
            {
                _director = Director.Value.GetComponent<IAttackDirector>();
            }

            _waitTime = UnityEngine.Random.Range(MinHesitationTime.Value, MaxHesitationTime.Value);
            _timerStart = Time.time;

            _enemyMovement.SetMovementIntent(Vector2.zero);
            LockFacingDirection();

            EventBus.Publish(new AINodeStateMessage(Agent.Value, "Hesitate", "Pensando..."));
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            LockFacingDirection();

            _enemyMovement.SetMovementIntent(Vector2.zero);

            if (Time.time - _timerStart < _waitTime) return Status.Running;

            EventBus.Publish(new AINodeStateMessage(Agent.Value, "Hesitate", "Sucesso - Hesitação Concluída"));
            return Status.Success;
        }

        protected override void OnEnd()
        {
            if (_enemyMovement != null) _enemyMovement.SetMovementIntent(Vector2.zero);
            UnlockFacingDirection();
        }

        private void LockFacingDirection()
        {
            if (_director == null) return;
            if (_entityLocomotion == null) return;
            
            Transform playerTransform = _director.GetPlayerTransform();
            if (playerTransform == null) return;

            float directionX = playerTransform.position.x - Agent.Value.transform.position.x;
            _entityLocomotion.SetFacingDirectionLock(true);
            _entityLocomotion.ForceFacingDirectionX(directionX);
        }

        private void UnlockFacingDirection()
        {
            if (_entityLocomotion == null) return;
            _entityLocomotion.SetFacingDirectionLock(false);
        }
    }
}
