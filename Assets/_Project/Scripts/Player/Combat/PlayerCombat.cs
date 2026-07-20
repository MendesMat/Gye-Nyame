using System.Collections.Generic;
using UnityEngine;
using GyeNyame.Core.InputBuffer;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Events;
using GyeNyame.Core.StateMachine;
using GyeNyame.Entities.Combat.States;
using GyeNyame.Entities.Movement.States;
using GyeNyame.Combat.Data;
using GyeNyame.Combat.Components;

namespace GyeNyame.Player.Combat
{
    [RequireComponent(typeof(StateMachine))]
    [RequireComponent(typeof(InputBuffer))]
    public class PlayerCombat : MonoBehaviour, IPlayerCombatContext
    {
        [Header("Components")]
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private InputBuffer inputBuffer;

        [Header("Combo Starters")]
        [SerializeField] private AttackDataSO lightAttackStarter;
        [SerializeField] private AttackDataSO heavyAttackStarter;

        public bool IsCancelWindowOpen { get; private set; }
        public InputBuffer InputBuffer => inputBuffer;
        public AttackDataSO CurrentAttackData => _currentAttackData;

        private IEntityLocomotion _locomotionContext;
        private AttackDataSO _currentAttackData;
        private float _lastAttackTime = 0f;
        private float _cooldownEndTime = 0f;
        private readonly Dictionary<AttackDataSO, IHitbox> _hitboxMap = new();

        private void Awake()
        {
            _locomotionContext = GetComponent<IEntityLocomotion>();
            InitializeStateMachine();
            InitializeHitboxes();
        }

        private void InitializeHitboxes()
        {
            var hitboxes = GetComponentsInChildren<HitboxComponent>(true);
            foreach (var hitbox in hitboxes)
            {
                if (hitbox.BoundAttackData != null)
                {
                    _hitboxMap[hitbox.BoundAttackData] = hitbox;
                }
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerAttackLightMessage>(OnAttackLight);
            EventBus.Subscribe<PlayerAttackHeavyMessage>(OnAttackHeavy);
            EventBus.Subscribe<PlayerDashMessage>(OnDashMessage);
            EventBus.Subscribe<PlayerJumpMessage>(OnJumpMessage);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerAttackLightMessage>(OnAttackLight);
            EventBus.Unsubscribe<PlayerAttackHeavyMessage>(OnAttackHeavy);
            EventBus.Unsubscribe<PlayerDashMessage>(OnDashMessage);
            EventBus.Unsubscribe<PlayerJumpMessage>(OnJumpMessage);
        }

        private void Update()
        {
            if (_locomotionContext.HasMoveInput) ResetCombo();

            bool canCombo = stateMachine.CurrentState is EntityIdleState || stateMachine.CurrentState is EntityWalkState || IsCancelWindowOpen;
            
            if (!canCombo) return;

            if (!IsCancelWindowOpen && _currentAttackData != null && Time.time - _lastAttackTime > _currentAttackData.ComboWindowTime)
            {
                ResetCombo();
            }

            if (inputBuffer.HasCommand<PlayerAttackLightMessage>())
            {
                inputBuffer.ConsumeCommand<PlayerAttackLightMessage>();

                if (_currentAttackData == null)
                {
                    if (Time.time < _cooldownEndTime || lightAttackStarter == null) return;
                    _currentAttackData = lightAttackStarter;
                    stateMachine.ChangeState(stateMachine.GetOrCreateState<GenericEntityAttackState>());
                    return;
                }
                
                if (_currentAttackData.NextLightCombo == null) return;
                _currentAttackData = _currentAttackData.NextLightCombo;

                stateMachine.ChangeState(stateMachine.GetOrCreateState<GenericEntityAttackState>());
                return;
            }

            if (inputBuffer.HasCommand<PlayerAttackHeavyMessage>())
            {
                inputBuffer.ConsumeCommand<PlayerAttackHeavyMessage>();
                
                if (_currentAttackData == null)
                {
                    if (Time.time < _cooldownEndTime || heavyAttackStarter == null) return;
                    _currentAttackData = heavyAttackStarter;
                    stateMachine.ChangeState(stateMachine.GetOrCreateState<GenericEntityAttackState>());
                    return;
                }

                if (_currentAttackData.NextHeavyCombo == null) return;
                _currentAttackData = _currentAttackData.NextHeavyCombo;
                
                stateMachine.ChangeState(stateMachine.GetOrCreateState<GenericEntityAttackState>());
            }
        }

        private void ResetCombo()
        {
            if (_currentAttackData != null)
            {
                _cooldownEndTime = _lastAttackTime + _currentAttackData.CooldownTime;
            }
            _currentAttackData = null;
        }

        private void InitializeStateMachine()
        {
            stateMachine.RegisterState<GenericEntityAttackState>(new StateFactory<GenericEntityAttackState>
                (sm => new GenericEntityAttackState(sm, this, _locomotionContext)));
        }

        private void OnAttackLight(PlayerAttackLightMessage message)
        {
            float bufferTime = ResolveLightBufferTime();
            inputBuffer.BufferCommand<PlayerAttackLightMessage>(bufferTime);
        }

        private float ResolveLightBufferTime()
        {
            if (_currentAttackData != null && _currentAttackData.NextLightCombo != null) return _currentAttackData.NextLightCombo.BufferTime;
            if (lightAttackStarter != null) return lightAttackStarter.BufferTime;
            return 0.2f;
        }

        private void OnAttackHeavy(PlayerAttackHeavyMessage message)
        {
            float bufferTime = ResolveHeavyBufferTime();
            inputBuffer.BufferCommand<PlayerAttackHeavyMessage>(bufferTime);
        }

        private float ResolveHeavyBufferTime()
        {
            if (_currentAttackData != null && _currentAttackData.NextHeavyCombo != null) return _currentAttackData.NextHeavyCombo.BufferTime;
            if (heavyAttackStarter != null) return heavyAttackStarter.BufferTime;
            return 0.2f;
        }
        
        public void OpenCancelWindow() => IsCancelWindowOpen = true;
        
        public void CloseCancelWindow() => IsCancelWindowOpen = false;

        public void FinishAttack()
        {
            if (stateMachine.CurrentState is BaseEntityAttackState attackState)
            {
                _lastAttackTime = Time.time;
                attackState.OnAnimationFinish();
            }
        }

        public void OpenHitbox()
        {
            if (_currentAttackData == null) return;
            
            if (_hitboxMap.TryGetValue(_currentAttackData, out var hitbox))
            {
                hitbox.EnableHitbox();
            }
        }

        public void CloseHitbox()
        {
            if (_currentAttackData == null) return;
            if (_hitboxMap.TryGetValue(_currentAttackData, out var hitbox))
            {
                hitbox.DisableHitbox();
            }
        }

        private void OnDashMessage(PlayerDashMessage message) => HandleInterrupt();
        private void OnJumpMessage(PlayerJumpMessage message) => HandleInterrupt();

        private void HandleInterrupt()
        {
            ResetCombo();
            
            if (!IsCancelWindowOpen) return;

            if (stateMachine.CurrentState is BaseEntityAttackState attackState && attackState.AllowInterrupt)
            {
                IsCancelWindowOpen = false;
                inputBuffer.Clear();
                EventBus.Publish(new EndCombatMessage());
            }
        }
    }
}
