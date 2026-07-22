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
        name: "ApproachCombatSlot",
        story: "Agent [Agent] smoothly approaches dynamic slot from [Director]",
        category: "Action",
        id: "approach_slot_1")]
    public partial class ApproachCombatSlotAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Director;
        [SerializeReference] public BlackboardVariable<float> MinReactionDelay = new(0.4f);
        [SerializeReference] public BlackboardVariable<float> MaxReactionDelay = new(0.8f);
        [SerializeReference] public BlackboardVariable<float> SlotArrivalToleranceX = new(0.15f);
        [SerializeReference] public BlackboardVariable<float> SlotArrivalToleranceZ = new(0.05f);
        [SerializeReference] public BlackboardVariable<float> SpeedSmoothingDistance = new(0.5f);

        private IEnemyMovement _enemyMovement;
        private IEntityLocomotion _locomotion;
        private IAttackDirector _director;

        private float _lastRecalculationTime;
        private float _reactionDelay;
        private Vector3 _cachedTargetSlot;

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

            _cachedTargetSlot = _director.GetAvailablePositionSlot(Agent.Value);
            _reactionDelay = UnityEngine.Random.Range(MinReactionDelay.Value, MaxReactionDelay.Value);
            _lastRecalculationTime = Time.time;

            EventBus.Publish(new AINodeStateMessage(Agent.Value, "ApproachCombatSlot", "Iniciou Aproximação"));
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (_director.IsInAttackRange(Agent.Value))
            {
                _enemyMovement.SetMovementIntent(Vector2.zero);
                EventBus.Publish(new AINodeStateMessage(Agent.Value, "ApproachCombatSlot", "Sucesso - Já no Range"));
                return Status.Success;
            }

            RecalculateSlotIfReady();
            return NavigateTowardsCachedSlot();
        }

        protected override void OnEnd()
        {
            if (_locomotion != null) _locomotion.SetFacingDirectionLock(false);
            if (_enemyMovement == null) return;
            _enemyMovement.SetMovementIntent(Vector2.zero);
        }

        private Status NavigateTowardsCachedSlot()
        {
            Vector3 current = Agent.Value.transform.position;
            float xDiff = _cachedTargetSlot.x - current.x;
            float zDiff = _cachedTargetSlot.z - current.z;

            if (HasArrivedAtSlot(xDiff, zDiff))
            {
                _enemyMovement.SetMovementIntent(Vector2.zero);
                EventBus.Publish(new AINodeStateMessage(Agent.Value, "ApproachCombatSlot", "Sucesso - Chegou no Slot"));
                return Status.Success;
            }

            float distance = Vector3.Distance(
                new Vector3(current.x, 0, current.z),
                new Vector3(_cachedTargetSlot.x, 0, _cachedTargetSlot.z));
            float clampFactor = Mathf.Clamp01(distance / SpeedSmoothingDistance.Value);
            Vector2 direction2D = new Vector2(xDiff, zDiff).normalized * clampFactor;

            _enemyMovement.SetMovementIntent(direction2D);
            return Status.Running;
        }

        private bool HasArrivedAtSlot(float xDiff, float zDiff)
        {
            return Mathf.Abs(xDiff) < SlotArrivalToleranceX.Value
                && Mathf.Abs(zDiff) < SlotArrivalToleranceZ.Value;
        }

        private void RecalculateSlotIfReady()
        {
            if (Time.time - _lastRecalculationTime < _reactionDelay) return;

            _cachedTargetSlot = _director.GetAvailablePositionSlot(Agent.Value);
            _reactionDelay = UnityEngine.Random.Range(MinReactionDelay.Value, MaxReactionDelay.Value);
            _lastRecalculationTime = Time.time;

            Transform playerTransform = _director.GetPlayerTransform();
            if (playerTransform == null) return;

            Vector3 current = Agent.Value.transform.position;
            _locomotion.ForceFacingDirectionX(playerTransform.position.x - current.x);
        }
    }
}
