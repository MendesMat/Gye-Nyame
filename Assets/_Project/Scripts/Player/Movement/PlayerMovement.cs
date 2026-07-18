using UnityEngine;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.StateMachine;
using GyeNyame.Player.Movement.States;
using GyeNyame.Agents.Movement;

namespace GyeNyame.Player.Movement
{
    [RequireComponent(typeof(StateMachine))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : BaseEntityMovement, IPlayerMovementContext
    {
        [Header("Jump Properties")]
        [SerializeField] private float jumpForce = 15f;
        [SerializeField] private float jumpCooldown = 0.2f;

        [Header("Air Movement")]
        [SerializeField] private float airSpeedMultiplier = 0.5f;
        [SerializeField] private bool lockDepthDuringJump = true;

        [Header("Dash Properties")]
        [SerializeField] private float dashSpeedMultiplier = 3f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 1f;

        private float _lastLandTime;
        private bool _jumpRequested;
        private bool _dashRequested;
        private float _lastDashTime;

        public Vector2 CurrentMoveInput => currentMoveInput;
        public Vector2 FacingDirection => facingDirection;

        public float AirSpeedMultiplier => airSpeedMultiplier;
        public bool LockDepthDuringJump => lockDepthDuringJump;
        public float DashSpeedMultiplier => dashSpeedMultiplier;
        public float DashDuration => dashDuration;

        protected override void Awake()
        {
            base.Awake();
            InitializeStateMachine();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerMoveMessage>(OnPlayerMove);
            EventBus.Subscribe<PlayerJumpMessage>(OnPlayerJump);
            EventBus.Subscribe<PlayerDashMessage>(OnPlayerDash);
            EventBus.Subscribe<EndCombatMessage>(OnEndCombatMessage);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerMoveMessage>(OnPlayerMove);
            EventBus.Unsubscribe<PlayerJumpMessage>(OnPlayerJump);
            EventBus.Unsubscribe<PlayerDashMessage>(OnPlayerDash);
            EventBus.Unsubscribe<EndCombatMessage>(OnEndCombatMessage);
        }

        private void Start()
        {
            stateMachine.ChangeState(stateMachine.GetOrCreateState<PlayerIdleState>());
        }

        private void InitializeStateMachine()
        {
            stateMachine.RegisterState<PlayerIdleState>(new StateFactory<PlayerIdleState>(sm => new PlayerIdleState(sm, this)));
            stateMachine.RegisterState<PlayerWalkState>(new StateFactory<PlayerWalkState>(sm => new PlayerWalkState(sm, this)));
            stateMachine.RegisterState<PlayerJumpState>(new StateFactory<PlayerJumpState>(sm => new PlayerJumpState(sm, this)));
            stateMachine.RegisterState<PlayerFallState>(new StateFactory<PlayerFallState>(sm => new PlayerFallState(sm, this)));
            stateMachine.RegisterState<PlayerDashState>(new StateFactory<PlayerDashState>(sm => new PlayerDashState(sm, this)));
        }

        private void OnPlayerMove(PlayerMoveMessage message)
        {
            currentMoveInput = message.MoveInput;
            
            if (isFacingDirectionLocked) return;
            
            UpdateFacingDirection();
        }

        private void OnEndCombatMessage(EndCombatMessage message)
        {
            if (HasMoveInput)
            {
                stateMachine.ChangeState(stateMachine.GetOrCreateState<PlayerWalkState>());
                return;
            }
            
            stateMachine.ChangeState(stateMachine.GetOrCreateState<PlayerIdleState>());
        }

        private void OnPlayerJump(PlayerJumpMessage message) => _jumpRequested = true;

        public bool ConsumeJumpRequest()
        {
            if (!_jumpRequested) return false;

            _jumpRequested = false;
            return Time.time >= _lastLandTime + jumpCooldown;
        }

        public void ExecuteJump()
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
        }

        protected override void OnLanded()
        {
            base.OnLanded();
            _lastLandTime = Time.time;
        }

        private void OnPlayerDash(PlayerDashMessage message) => _dashRequested = true;

        public bool ConsumeDashRequest()
        {
            if (!_dashRequested) return false;

            _dashRequested = false;
            return Time.time >= _lastDashTime + dashCooldown;
        }

        public void ExecuteDash() => _lastDashTime = Time.time;

        public void UpdateDirectionalMovement(Vector2 direction, float speedMult)
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
