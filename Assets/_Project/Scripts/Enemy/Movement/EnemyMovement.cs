using UnityEngine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Entities.Movement;
using GyeNyame.Entities.Movement.States;
using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Enemy.Movement
{
    [RequireComponent(typeof(StateMachine))]
    public class EnemyMovement : BaseEntityMovement, IEnemyMovement
    {
        private IEntityHealth _health;

        protected override void Awake()
        {
            base.Awake();
            _health = GetComponentInChildren<IEntityHealth>();
            InitializeStateMachine();
        }

        private void Start()
        {
            stateMachine.ChangeState(stateMachine.GetOrCreateState<EntityIdleState>());
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EndCombatMessage>(OnEndCombatMessage);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EndCombatMessage>(OnEndCombatMessage);
        }

        private void OnEndCombatMessage(EndCombatMessage message)
        {
            if (message.Entity != gameObject) return;
            if (_health != null && _health.IsDead) return;
            
            if (HasMoveInput)
                stateMachine.ChangeState(stateMachine.GetOrCreateState<EntityWalkState>());
            else
                stateMachine.ChangeState(stateMachine.GetOrCreateState<EntityIdleState>());
        }

        private void InitializeStateMachine()
        {
            stateMachine.RegisterState<EntityIdleState>(new StateFactory<EntityIdleState>(sm => new EntityIdleState(sm, this)));
            stateMachine.RegisterState<EntityWalkState>(new StateFactory<EntityWalkState>(sm => new EntityWalkState(sm, this)));
            stateMachine.RegisterState<EntityFallState>(new StateFactory<EntityFallState>(sm => new EntityFallState(sm, this)));
        }

        public void SetMovementIntent(Vector2 direction)
        {
            currentMoveInput = direction;

            if (isFacingDirectionLocked) return;
            UpdateFacingDirection();
        }

        private void Update()
        {
            if (_health != null && _health.IsDead) return;

            if (HasMoveInput && stateMachine.CurrentState is EntityIdleState)
            {
                stateMachine.ChangeState(stateMachine.GetOrCreateState<EntityWalkState>());
                return;
            }

            if (!HasMoveInput && stateMachine.CurrentState is EntityWalkState)
            {
                stateMachine.ChangeState(stateMachine.GetOrCreateState<EntityIdleState>());
            }
        }
        
        public override void UpdateDirectionalMovement(Vector2 direction, float speedMult)
        {
            var currentPosition = rigidBody.position;
            var directionMovement = new Vector3(direction.x, 0f, direction.y * depthSpeedMultiplier) * speedMult;
            var intendedMovement = directionMovement * speed * Time.fixedDeltaTime;

            var allowedMovement = kinematicPhysics.CalculateAllowedMovement(currentPosition, intendedMovement);
            var targetPosition = currentPosition + allowedMovement;

            targetPosition = ApplyVerticalMovement(targetPosition);
            rigidBody.MovePosition(targetPosition);
        }
    }
}
