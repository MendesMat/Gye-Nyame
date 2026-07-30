using System.Collections.Generic;
using UnityEngine;
using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Entities.Animation
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class EntityAnimationHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected Animator animator;
        [SerializeField] protected SpriteRenderer spriteRenderer;

        protected IStateMachine stateMachine;
        protected IEntityLocomotion locomotionContext;
        
        protected readonly Dictionary<EntityStateCategory, int> categoryToHash = new();
        protected float facingDirectionX = 1f;

        protected virtual void Awake()
        {
            stateMachine = GetComponentInParent<IStateMachine>();
            locomotionContext = GetComponentInParent<IEntityLocomotion>();
            SetupAnimator();
        }

        protected virtual void OnEnable()
        {
            if (stateMachine != null) stateMachine.OnStateChanged += PlayAnimationForState;
        }

        protected virtual void OnDisable()
        {
            if (stateMachine != null) stateMachine.OnStateChanged -= PlayAnimationForState;
        }

        protected virtual void Update()
        {
            if (locomotionContext == null || locomotionContext.FacingDirectionX == 0f) return;
            
            facingDirectionX = locomotionContext.FacingDirectionX;
            FlipSpriteTowardsFacingDirection();
        }

        protected virtual void SetupAnimator()
        {
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogWarning("Animator is missing a controller.", this);
                return;
            }

            InitializeAnimations();
        }

        protected virtual void InitializeAnimations()
        {
            categoryToHash[EntityStateCategory.Idle] = Animator.StringToHash("Idle");
            categoryToHash[EntityStateCategory.Walk] = Animator.StringToHash("Walk");
            categoryToHash[EntityStateCategory.Jump] = Animator.StringToHash("Jump");
            categoryToHash[EntityStateCategory.Fall] = Animator.StringToHash("Fall");
            categoryToHash[EntityStateCategory.Dash] = Animator.StringToHash("Dash");
            categoryToHash[EntityStateCategory.AttackLight1] = Animator.StringToHash("AttackLight1");
            categoryToHash[EntityStateCategory.AttackLight2] = Animator.StringToHash("AttackLight2");
            categoryToHash[EntityStateCategory.AttackHeavy] = Animator.StringToHash("AttackHeavy");
            categoryToHash[EntityStateCategory.Hurt] = Animator.StringToHash("Hurt");
            categoryToHash[EntityStateCategory.Dead] = Animator.StringToHash("Dead");
        }

        protected virtual void PlayAnimationForState(BaseState previous, BaseState next)
        {
            if (stateMachine.CurrentState != next) return;

            if (TryPlayStandardAnimation(next)) return;
            
            HandleCustomAnimation(next);
        }

        protected bool TryPlayStandardAnimation(BaseState state)
        {
            if (state.StateCategory == EntityStateCategory.None) return false;
            
            if (!categoryToHash.TryGetValue(state.StateCategory, out int animationHash)) return false;
            
            animator.Play(animationHash, -1, 0f);
            return true;
        }

        protected virtual void HandleCustomAnimation(BaseState state) { }

        protected virtual void FlipSpriteTowardsFacingDirection()
        {
            float yRotation = facingDirectionX < 0f ? 180f : 0f;
            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        public void OnDeathAnimationFinish()
        {
            var targetGo = (stateMachine as MonoBehaviour)?.gameObject ?? gameObject;
            EventBus.Publish(new AnimationFinishDeathMessage(targetGo));
        }
    }
}
