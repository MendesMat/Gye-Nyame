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
        name: "PaceAroundSlot", 
        story: "Agent [Agent] paces randomly around slot from [Director]", 
        category: "Action", 
        id: "pace_around_slot_1")]
    public partial class PaceAroundSlotAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Director;

        private IEnemyMovement _enemyMovement;
        private IEntityLocomotion _locomotion;
        private IAttackDirector _director;
        private int _enemyId;
        private Vector3 _targetWanderPoint;
        private float _timeStarted;

        protected override Status OnStart()
        {
            if (Agent.Value == null) return Status.Failure;
            if (Director.Value == null) return Status.Failure;

            _enemyMovement = Agent.Value.GetComponent<IEnemyMovement>();
            if (_enemyMovement == null) return Status.Failure;

            _locomotion = Agent.Value.GetComponent<IEntityLocomotion>();
            if (_locomotion == null) return Status.Failure;
            _locomotion.SetFacingDirectionLock(true);

            _director = Director.Value.GetComponent<IAttackDirector>();
            if (_director == null) return Status.Failure;

            Vector3 baseSlot = _director.GetAvailablePositionSlot(Agent.Value);
            float randomX = UnityEngine.Random.Range(-0.6f, 0.6f);
            _targetWanderPoint = baseSlot + new Vector3(randomX, 0f, 0f);
            _timeStarted = Time.time;

            EventBus.Publish(new AINodeStateMessage(Agent.Value, "PaceAroundSlot", "Iniciou Hesitação/Pacing"));
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            Vector3 current = Agent.Value.transform.position;
            Transform playerTransform = _director.GetPlayerTransform();
            
            if (playerTransform != null)
            {
                _locomotion.ForceFacingDirectionX(playerTransform.position.x - current.x);
            }

            if (Time.time - _timeStarted > 1.5f)
            {
                _enemyMovement.SetMovementIntent(Vector2.zero);
                EventBus.Publish(new AINodeStateMessage(Agent.Value, "PaceAroundSlot", "Sucesso - Tempo Esgotado"));
                return Status.Success;
            }

            float distance = Vector3.Distance(current, _targetWanderPoint);
            
            if (distance < 0.1f)
            {
                _enemyMovement.SetMovementIntent(Vector2.zero);
                EventBus.Publish(new AINodeStateMessage(Agent.Value, "PaceAroundSlot", "Sucesso - Chegou no Ponto"));
                return Status.Success;
            }

            Vector3 direction3D = (_targetWanderPoint - current).normalized;
            _enemyMovement.SetMovementIntent(new Vector2(direction3D.x, direction3D.z));
            
            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (_locomotion != null) _locomotion.SetFacingDirectionLock(false);
            if (_enemyMovement == null) return;
            _enemyMovement.SetMovementIntent(Vector2.zero);
        }
    }
}
