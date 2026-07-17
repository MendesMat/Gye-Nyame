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
        [SerializeField] private MonoBehaviour kinematicPhysicsComponent;

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

        private IKinematicPhysics _kinematicPhysics;
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
            if(kinematicPhysicsComponent is not IKinematicPhysics physics)
            {
                Debug.LogError("kinematicPhysicsComponent does not implement IKinematicPhysics!");
                return;
            }
            
            _kinematicPhysics = physics;

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

            if (!isLocked)
            {
                if (_currentMoveInput.x != 0f)
                {
                    _facingDirectionX = Mathf.Sign(_currentMoveInput.x);
                }

                if (_currentMoveInput != Vector2.zero)
                {
                    _facingDirection = _currentMoveInput.normalized;
                }
            }
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
            var currentPosition = rigidBody.position;

            var depthInput = lockDepth ? 0f : _currentMoveInput.y;
            var directionMovement = new Vector3(_currentMoveInput.x, 0f, depthInput * depthSpeedMultiplier) * speedMultiplier;
            var intendedMovement = directionMovement * speed * Time.fixedDeltaTime;

            var allowedMovement = _kinematicPhysics.CalculateAllowedMovement(currentPosition, intendedMovement);
            var targetPosition = currentPosition + allowedMovement;

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

        private Vector3 ApplyVerticalMovement(Vector3 currentPosition)
        {
            if (!_isGrounded) 
            {
                _verticalVelocity -= gravity * Time.fixedDeltaTime;
            }
            
            float intendedFall = _verticalVelocity * Time.fixedDeltaTime;

            if (intendedFall < 0f)
            {
                float fallDistance = Mathf.Abs(intendedFall);
                if (_kinematicPhysics.CheckGround(currentPosition, fallDistance, out float allowedFall))
                {
                    _verticalVelocity = 0f;
                    _isGrounded = true;
                    _lastLandTime = Time.time;
                    currentPosition.y -= allowedFall;
                    return currentPosition;
                }
            }
            
            else if (_isGrounded)
            {
                if (!_kinematicPhysics.CheckGround(currentPosition, 0.05f, out _))
                {
                    _isGrounded = false;
                }
            }

            currentPosition.y += intendedFall;
            return currentPosition;
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
            var currentPosition = rigidBody.position;

            var directionMovement = new Vector3(direction.x, 0f, direction.y * depthSpeedMultiplier) * speedMultiplier;
            var intendedMovement = directionMovement * speed * Time.fixedDeltaTime;

            var allowedMovement = _kinematicPhysics.CalculateAllowedMovement(currentPosition, intendedMovement);
            var targetPosition = currentPosition + allowedMovement;

            targetPosition = ApplyVerticalMovement(targetPosition);
            rigidBody.MovePosition(targetPosition);
        }
        #endregion
    }
}

