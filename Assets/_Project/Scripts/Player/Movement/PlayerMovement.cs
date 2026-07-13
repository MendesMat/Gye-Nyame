using UnityEngine;
using GyeNyame.Core.EventBus;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.StateMachine;
using GyeNyame.Player.Movement.States;

namespace GyeNyame.Player.Movement
{
    [RequireComponent(typeof(StateMachine))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour, IPlayerMovementContext, IPlayerLocomotion
    {
        [Header("Components")]
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private Rigidbody rigidBody;

        [Header("Movement Properties")]
        [SerializeField] private float speed;
        [SerializeField] private float depthSpeedMultiplier;

        [Header("Jump Properties")]
        [SerializeField] private float gravity;
        [SerializeField] private float jumpForce;

        [Header("Air Movement")]
        [SerializeField] private float airSpeedMultiplier = 0.5f;
        [SerializeField] private bool lockDepthDuringJump = true;

        [Header("Dash Properties")]
        [SerializeField] private float dashSpeedMultiplier = 3f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 1f;

        [Header("Cooldowns")]
        [SerializeField] private float jumpCooldown = 0.2f;

        private float _groundYPosition;
        private bool _isGrounded;
        private float _verticalVelocity;
        private Vector2 _currentMoveInput;
        private float _lastLandTime;
        private bool _jumpRequested;
        private bool _dashRequested;
        private float _lastDashTime;
        private Vector2 _facingDirection = Vector2.right;
        private float _facingDirectionX;
        private bool _isFacingDirectionLocked;

        // IPlayerMovementContext
        public bool HasMoveInput => _currentMoveInput != Vector2.zero;
        public bool IsGrounded => _isGrounded;
        public Vector2 CurrentMoveInput => _currentMoveInput;
        public Vector2 FacingDirection => _facingDirection;
        public float FacingDirectionX => _facingDirectionX;

        public float AirSpeedMultiplier => airSpeedMultiplier;
        public bool LockDepthDuringJump => lockDepthDuringJump;
        public float DashSpeedMultiplier => dashSpeedMultiplier;
        public float DashDuration => dashDuration;

        #region Lifecycle
        private void Awake()
        {
            _groundYPosition = transform.position.y;
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
            stateMachine.RegisterState<PlayerDashState>(new StateFactory<PlayerDashState>(sm => new PlayerDashState(sm, this)));
        }
        #endregion

        #region Horizontal Movement
        private void OnPlayerMove(PlayerMoveMessage message)
        {
            _currentMoveInput = message.MoveInput;

            if (_currentMoveInput.x != 0f && !_isFacingDirectionLocked)
            {
                _facingDirectionX = Mathf.Sign(_currentMoveInput.x);
            }

            if (_currentMoveInput != Vector2.zero && !_isFacingDirectionLocked)
            {
                _facingDirection = _currentMoveInput.normalized;
            }
        }

        public void SetFacingDirectionLock(bool isLocked)
        {
            _isFacingDirectionLocked = isLocked;
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

        public void UpdateMovement(float speedMultiplier, bool lockDepth = false)
        {
            var targetPosition = rigidBody.position;

            var depthInput = lockDepth ? 0f : _currentMoveInput.y;
            var horizontalMovement = new Vector3(_currentMoveInput.x, 0f, depthInput * depthSpeedMultiplier) * speedMultiplier;
            targetPosition += horizontalMovement * speed * Time.fixedDeltaTime;

            targetPosition = ApplyVerticalMovement(targetPosition);
            rigidBody.MovePosition(targetPosition);
        }
        #endregion

        #region Jump Mechanic
        private void OnPlayerJump(PlayerJumpMessage message) => _jumpRequested = true;

        public bool ConsumeJumpRequest()
        {
            if (!_jumpRequested) return false;

            _jumpRequested = false;
            return Time.time >= _lastLandTime + jumpCooldown;
        }

        public void ExecuteJump()
        {
            _verticalVelocity = jumpForce;
            _isGrounded = false;
        }

        private Vector3 ApplyVerticalMovement(Vector3 targetPosition)
        {
            if (!_isGrounded) _verticalVelocity -= gravity * Time.fixedDeltaTime;
            targetPosition.y += _verticalVelocity * Time.fixedDeltaTime;

            if (ShouldLand(targetPosition)) targetPosition = Land(targetPosition);

            return targetPosition;
        }

        private bool ShouldLand(Vector3 targetPosition) =>
            !_isGrounded && _verticalVelocity <= 0f && targetPosition.y <= _groundYPosition;

        private Vector3 Land(Vector3 targetPosition)
        {
            _verticalVelocity = 0f;
            _isGrounded = true;
            _lastLandTime = Time.time;
            targetPosition.y = _groundYPosition;
            return targetPosition;
        }
        #endregion

        #region Dash Mechanic
        private void OnPlayerDash(PlayerDashMessage message) => _dashRequested = true;

        public bool ConsumeDashRequest()
        {
            if (!_dashRequested) return false;

            _dashRequested = false;
            return Time.time >= _lastDashTime + dashCooldown;
        }

        public void ExecuteDash() => _lastDashTime = Time.time;

        public void UpdateDirectionalMovement(Vector2 direction, float speedMultiplier)
        {
            var targetPosition = rigidBody.position;

            var horizontalMovement = new Vector3(direction.x, 0f, direction.y * depthSpeedMultiplier) * speedMultiplier;
            targetPosition += horizontalMovement * speed * Time.fixedDeltaTime;

            targetPosition = ApplyVerticalMovement(targetPosition);
            rigidBody.MovePosition(targetPosition);
        }
        #endregion
    }
}

