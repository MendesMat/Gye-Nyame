using UnityEngine;
using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Entities.Combat.States
{
    public class EntityDeadState : BaseState
    {
        protected readonly IEntityLocomotion locomotionContext;
        protected readonly IEntityHealth healthContext;
        protected readonly IEntityVisuals visualsContext;

        private bool _isFading;
        private float _fadeTimer;

        public override EntityStateCategory StateCategory => EntityStateCategory.Dead;

        public EntityDeadState(IStateMachine stateMachine, IEntityLocomotion locomotionContext, IEntityHealth healthContext, IEntityVisuals visualsContext) 
            : base(stateMachine)
        {
            this.locomotionContext = locomotionContext;
            this.healthContext = healthContext;
            this.visualsContext = visualsContext;
        }

        public override void Enter()
        {
            _isFading = false;
            locomotionContext?.SetFacingDirectionLock(true);
            locomotionContext?.DisablePhysics();
            healthContext?.SetDeadLayer(true);
            EventBus.Subscribe<AnimationFinishDeathMessage>(OnAnimationFinish);
        }

        public override void Exit()
        {
            EventBus.Unsubscribe<AnimationFinishDeathMessage>(OnAnimationFinish);
            healthContext?.SetDeadLayer(false);
            visualsContext?.ResetVisuals();
            base.Exit();
        }

        public override void Update()
        {
            if (!_isFading) return;
            
            _fadeTimer -= Time.deltaTime;
            float duration = visualsContext?.FadeDuration ?? 1f;
            float alpha = Mathf.Clamp01(_fadeTimer / duration);
            
            visualsContext?.SetAlpha(alpha);
            
            if (_fadeTimer > 0f) return;
            
            _isFading = false;
            FinishDeath();
        }

        private void OnAnimationFinish(AnimationFinishDeathMessage message)
        {
            var go = (StateMachine as MonoBehaviour)?.gameObject;
            if (go == null || message.Target != go) return;
            
            _isFading = true;
            _fadeTimer = visualsContext?.FadeDuration ?? 1f;
        }

        public virtual void FinishDeath()
        {
            var go = (StateMachine as MonoBehaviour)?.gameObject;
            if (go != null) go.SetActive(false);
        }
    }
}
