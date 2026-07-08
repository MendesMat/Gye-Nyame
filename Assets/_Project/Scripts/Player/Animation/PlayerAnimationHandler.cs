using System;
using System.Collections.Generic;
using GyeNyame.Core.EventBus;
using GyeNyame.Core.StateMachine;
using GyeNyame.Player.Contracts.Messages;
using GyeNyame.Player.Movement.States;
using UnityEngine;

namespace GyeNyame.Player.Animation
{
    public class PlayerAnimationHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Animation Clips")]
        [SerializeField] private AnimationClip idleClip;
        [SerializeField] private AnimationClip walkClip;
        [SerializeField] private AnimationClip jumpClip;
        [SerializeField] private AnimationClip dashClip;

        private IStateMachine _stateMachine;
        private Dictionary<Type, int> _stateToAnimationHash;

        private float _facingDirectionX = 1f;

        // Nomes genéricos e imutáveis dos estados no Animator Base Controller
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int DashHash = Animator.StringToHash("Dash");

        #region Lifecycle
        private void Awake()
        {
            _stateMachine = GetComponentInParent<IStateMachine>();
            SetupOverrideController();
            _stateToAnimationHash = BuildAnimationMap();
        }

        private void OnEnable()
        {
            if (_stateMachine != null)
                _stateMachine.OnStateChanged += PlayAnimationForState;

            EventBus.Subscribe<PlayerMoveMessage>(OnPlayerMove);
        }

        private void OnDisable()
        {
            if (_stateMachine != null)
                _stateMachine.OnStateChanged -= PlayAnimationForState;

            EventBus.Unsubscribe<PlayerMoveMessage>(OnPlayerMove);
        }

        private void Update()
        {
            FlipSpriteTowardsFacingDirection();
        }
        #endregion

        #region Setup
        private void SetupOverrideController()
        {
            var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

            overrideController["Idle"] = idleClip;
            overrideController["Walk"] = walkClip;
            overrideController["Jump"] = jumpClip;
            overrideController["Dash"] = dashClip;

            animator.runtimeAnimatorController = overrideController;
        }

        private Dictionary<Type, int> BuildAnimationMap() => new()
        {
            { typeof(PlayerIdleState), IdleHash },
            { typeof(PlayerWalkState), WalkHash },
            { typeof(PlayerJumpState), JumpHash },
            { typeof(PlayerDashState), DashHash },
        };
        #endregion

        #region Event Handlers
        private void OnPlayerMove(PlayerMoveMessage message)
        {
            if (message.MoveInput.x != 0f) _facingDirectionX = message.MoveInput.x;
        }
        #endregion

        #region Animation
        private void PlayAnimationForState(BaseState previous, BaseState next)
        {
            if (_stateToAnimationHash.TryGetValue(next.GetType(), out int animationHash))
                animator.Play(animationHash);
        }
        #endregion

        #region Visual
        private void FlipSpriteTowardsFacingDirection()
        {
            spriteRenderer.flipX = _facingDirectionX < 0f;
        }
        #endregion
    }
}
