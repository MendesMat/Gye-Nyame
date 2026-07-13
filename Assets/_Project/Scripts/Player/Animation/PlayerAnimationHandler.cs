using System;
using System.Collections.Generic;
using GyeNyame.Core.EventBus;
using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Messages;
using UnityEngine;

namespace GyeNyame.Player.Animation
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAnimationHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private IStateMachine _stateMachine;
        private readonly Dictionary<string, int> _stateToHash = new();

        private float _facingDirectionX = 1f;

        #region Animation Constants
        private const string StateIdle = "PlayerIdleState";
        private const string StateWalk = "PlayerWalkState";
        private const string StateJump = "PlayerJumpState";
        private const string StateDash = "PlayerDashState";
        private const string StateAttackLight1 = "PlayerAttackLight1State";
        private const string StateAttackLight2 = "PlayerAttackLight2State";
        private const string StateAttackHeavy = "PlayerAttackHeavyState";

        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int DashHash = Animator.StringToHash("Dash");
        private static readonly int AttackLight1Hash = Animator.StringToHash("AttackLight1");
        private static readonly int AttackLight2Hash = Animator.StringToHash("AttackLight2");
        private static readonly int AttackHeavyHash = Animator.StringToHash("AttackHeavy");
        #endregion

        #region Lifecycle
        private void Awake()
        {
            _stateMachine = GetComponentInParent<IStateMachine>();
            SetupAnimator();
        }

        private void OnEnable()
        {
            if (_stateMachine != null) _stateMachine.OnStateChanged += PlayAnimationForState;
            EventBus.Subscribe<PlayerMoveMessage>(OnPlayerMove);
        }

        private void OnDisable()
        {
            if (_stateMachine != null) _stateMachine.OnStateChanged -= PlayAnimationForState;
            EventBus.Unsubscribe<PlayerMoveMessage>(OnPlayerMove);
        }
        #endregion

        #region Setup
        private void SetupAnimator()
        {
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogWarning("Animator is missing a controller.", this);
                return;
            }

            InitializeAnimations();
        }

        private void InitializeAnimations()
        {
            _stateToHash[StateIdle] = IdleHash;
            _stateToHash[StateWalk] = WalkHash;
            _stateToHash[StateJump] = JumpHash;
            _stateToHash[StateDash] = DashHash;
            _stateToHash[StateAttackLight1] = AttackLight1Hash;
            _stateToHash[StateAttackLight2] = AttackLight2Hash;
            _stateToHash[StateAttackHeavy] = AttackHeavyHash;
        }
        #endregion

        #region Event Handlers
        private void OnPlayerMove(PlayerMoveMessage message)
        {
            if (message.MoveInput.x != 0f) 
            {
                _facingDirectionX = message.MoveInput.x;
                FlipSpriteTowardsFacingDirection();
            }
        }
        #endregion

        #region Animation
        private void PlayAnimationForState(BaseState previous, BaseState next)
        {
            if (_stateToHash.TryGetValue(next.StateName, out int animationHash))
            {
                animator.Play(animationHash);
            }
        }
        #endregion

        #region Visual
        private void FlipSpriteTowardsFacingDirection()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = _facingDirectionX < 0f;
            }
        }
        #endregion
    }
}
