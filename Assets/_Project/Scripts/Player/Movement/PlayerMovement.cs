using UnityEngine;
using GyeNyame.Core.EventBus;
using GyeNyame.Player.Contracts.Messages;
using GyeNyame.Core.StateMachine;
using GyeNyame.Player.Movement.States;

namespace GyeNyame.Player.Movement
{
    [RequireComponent(typeof(StateMachine))]
    public class PlayerMovement : MonoBehaviour, IPlayerMovementContext
    {
        [Header("Components")]
        [SerializeField] private Rigidbody rb;

        [Header("Movement Properties")]
        [SerializeField] private float speed;
        [SerializeField] private float depthSpeedMultiplier;

        [Header("Jump Properties")]
        [SerializeField] private float gravity;
        [SerializeField] private float jumpForce;

        [Header("Air Movement")]
        [SerializeField] private float airSpeedMultiplier = 0.5f;
        [SerializeField] private bool lockDepthDuringJump = true;

        [Header("Cooldowns")]
        [SerializeField] private float jumpCooldown = 0.2f;

        private float _groundYPosition;
        private bool _isGrounded;
        private float _verticalVelocity;
        private Vector2 _currentMoveInput;
        private float _lastLandTime;
        private bool _jumpRequested;

        private StateMachine _stateMachine;

        // IPlayerMovementContext
        public bool HasMoveInput => _currentMoveInput != Vector2.zero;
        public bool IsGrounded => _isGrounded;
        public float AirSpeedMultiplier => airSpeedMultiplier;
        public bool LockDepthDuringJump => lockDepthDuringJump;

        private void Awake()
        {
            _groundYPosition = transform.position.y;
            _stateMachine = GetComponent<StateMachine>();
            InitializeStateMachine();
        }

        private void Start()
        {
            _stateMachine.ChangeState(_stateMachine.GetOrCreateState<PlayerIdleState>());
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerMoveMessage>(OnPlayerMove);
            EventBus.Subscribe<PlayerJumpMessage>(OnPlayerJump);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerMoveMessage>(OnPlayerMove);
            EventBus.Unsubscribe<PlayerJumpMessage>(OnPlayerJump);
        }

        // Event Handlers
        private void OnPlayerMove(PlayerMoveMessage message) => _currentMoveInput = message.MoveInput;
        private void OnPlayerJump(PlayerJumpMessage message) => _jumpRequested = true;

        // Initialization
        private void InitializeStateMachine()
        {
            _stateMachine.RegisterState<PlayerIdleState>(new StateFactory<PlayerIdleState>(sm => new PlayerIdleState(sm, this)));
            _stateMachine.RegisterState<PlayerWalkState>(new StateFactory<PlayerWalkState>(sm => new PlayerWalkState(sm, this)));
            _stateMachine.RegisterState<PlayerJumpState>(new StateFactory<PlayerJumpState>(sm => new PlayerJumpState(sm, this)));
        }

        // IPlayerMovementContext
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

        public void UpdateMovement(float speedMultiplier, bool lockDepth)
        {
            var calculatedTargetPosition = rb.position;

            // Horizontal Movement
            var depthInput = lockDepth ? 0f : _currentMoveInput.y;
            var horizontalMovement = new Vector3(_currentMoveInput.x, 0f, depthInput * depthSpeedMultiplier) * speedMultiplier;
            calculatedTargetPosition += horizontalMovement * speed * Time.fixedDeltaTime;

            // Vertical Movement
            if (!_isGrounded) _verticalVelocity -= gravity * Time.fixedDeltaTime;
            calculatedTargetPosition.y += _verticalVelocity * Time.fixedDeltaTime;

            // Collision / Landing Check
            if (!_isGrounded && _verticalVelocity <= 0f && calculatedTargetPosition.y <= _groundYPosition)
            {
                _verticalVelocity = 0f;
                _isGrounded = true;
                _lastLandTime = Time.time;
                calculatedTargetPosition.y = _groundYPosition;
            }

            // Apply Physics
            rb.MovePosition(calculatedTargetPosition);
        }
    }
}
