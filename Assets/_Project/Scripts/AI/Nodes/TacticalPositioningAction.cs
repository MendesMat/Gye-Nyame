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
        name: "TacticalPositioning",
        story: "Agent [Agent] repositions tactically away from [Director]",
        category: "Action",
        id: "tactical_positioning_1")]
    public partial class TacticalPositioningAction : Action
    {
        private const float ZeroValue = 0f;
        private const float DefaultDirectionX = 1f;
        private const float DefaultMinLateralOffset = -3f;
        private const float DefaultMaxLateralOffset = 3f;
        
        private const string EventStateStarted = "Tactical positioning started";
        private const string EventStateTimeout = "Success - Time elapsed";
        private const string EventStateArrived = "Success - Reached destination";

        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Director;
        [SerializeReference] public BlackboardVariable<float> RepositionDuration = new();
        [SerializeReference] public BlackboardVariable<float> StandbyDistance = new();
        [SerializeReference] public BlackboardVariable<float> ArrivalTolerance = new();
        [SerializeReference] public BlackboardVariable<float> MinLateralOffset = new(DefaultMinLateralOffset);
        [SerializeReference] public BlackboardVariable<float> MaxLateralOffset = new(DefaultMaxLateralOffset);

        private IEnemyMovement _enemyMovement;
        private IEntityLocomotion _entityLocomotion;
        private IAttackDirector _director;
        
        private Vector3 _tacticalDestination;
        private float _timeStarted;
        private float _lateralOffset;

        protected override Status OnStart()
        {
            if (IsInvalidConfiguration()) return Status.Failure;

            InitializeDependencies();
            
            if (HasMissingDependencies()) return Status.Failure;

            InitializeState();
            LockFacingDirection();
            PublishState(EventStateStarted);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            LockFacingDirection();

            if (IsRepositionTimeElapsed()) return HandleTimeout();

            UpdateTacticalDestination();
            
            if (HasArrivedAtDestination()) return HandleArrival();

            MoveTowardsDestination();

            return Status.Running;
        }

        protected override void OnEnd()
        {
            StopMovement();
            UnlockFacingDirection();
        }

        private bool IsInvalidConfiguration()
        {
            if (Agent.Value == null) return true;
            if (Director.Value == null) return true;
            
            return false;
        }

        private void InitializeDependencies()
        {
            _enemyMovement = Agent.Value.GetComponent<IEnemyMovement>();
            _entityLocomotion = Agent.Value.GetComponent<IEntityLocomotion>();
            _director = Director.Value.GetComponent<IAttackDirector>();
        }

        private bool HasMissingDependencies()
        {
            if (_enemyMovement == null) return true;
            if (_entityLocomotion == null) return true;
            if (_director == null) return true;
            
            return false;
        }

        private void InitializeState()
        {
            _timeStarted = Time.time;
            CalculateLateralOffset();
            UpdateTacticalDestination();
        }

        private void CalculateLateralOffset()
        {
            Transform playerTransform = _director.GetPlayerTransform();
            if (playerTransform == null) return;

            float currentAgentZ = Agent.Value.transform.position.z;
            float currentPlayerZ = playerTransform.position.z;
            
            bool isAbovePlayer = currentAgentZ > currentPlayerZ;
            
            if (isAbovePlayer)
            {
                _lateralOffset = UnityEngine.Random.Range(MinLateralOffset.Value, ZeroValue);
                return;
            }

            _lateralOffset = UnityEngine.Random.Range(ZeroValue, MaxLateralOffset.Value);
        }

        private void LockFacingDirection()
        {
            Transform playerTransform = _director.GetPlayerTransform();
            
            if (playerTransform == null) return;

            Vector3 agentPosition = Agent.Value.transform.position;
            float directionX = playerTransform.position.x - agentPosition.x;

            _entityLocomotion.SetFacingDirectionLock(true);
            _entityLocomotion.ForceFacingDirectionX(directionX);
        }

        private void UnlockFacingDirection()
        {
            _entityLocomotion.SetFacingDirectionLock(false);
        }

        private bool IsRepositionTimeElapsed()
        {
            return Time.time - _timeStarted > RepositionDuration.Value;
        }

        private Status HandleTimeout()
        {
            StopMovement();
            PublishState(EventStateTimeout);
            
            return Status.Success;
        }

        private void UpdateTacticalDestination()
        {
            Transform playerTransform = _director.GetPlayerTransform();
            
            if (playerTransform == null) return;

            Vector3 agentPosition = Agent.Value.transform.position;
            Vector3 playerPosition = playerTransform.position;
            
            float directionX = Mathf.Sign(agentPosition.x - playerPosition.x);
            if (directionX == ZeroValue) directionX = DefaultDirectionX;

            float targetX = playerPosition.x + (directionX * StandbyDistance.Value);
            float targetZ = playerPosition.z + _lateralOffset;

            _tacticalDestination = new Vector3(targetX, agentPosition.y, targetZ);
        }

        private bool HasArrivedAtDestination()
        {
            Vector3 currentPosition = Agent.Value.transform.position;
            float distance = Vector3.Distance(currentPosition, _tacticalDestination);
            
            return distance < ArrivalTolerance.Value;
        }

        private Status HandleArrival()
        {
            StopMovement();
            PublishState(EventStateArrived);
            return Status.Success;
        }

        private void MoveTowardsDestination()
        {
            Vector3 currentPosition = Agent.Value.transform.position;
            Vector3 direction = (_tacticalDestination - currentPosition).normalized;
            
            _enemyMovement.SetMovementIntent(new Vector2(direction.x, direction.z));
        }

        private void StopMovement()
        {
            _enemyMovement.SetMovementIntent(Vector2.zero);
        }

        private void PublishState(string message)
        {
            EventBus.Publish(new AINodeStateMessage(Agent.Value, nameof(TacticalPositioningAction), message));
        }
    }
}
