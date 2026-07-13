using UnityEngine;
using GyeNyame.Core.InputBuffer;
using GyeNyame.Core.Contracts.Messages;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.EventBus;
using GyeNyame.Core.StateMachine;
using GyeNyame.Player.Combat.States;

namespace GyeNyame.Player.Combat
{
    [RequireComponent(typeof(StateMachine))]
    [RequireComponent(typeof(InputBuffer))]
    public class PlayerCombat : MonoBehaviour, IPlayerCombatContext
    {
        [Header("Components")]
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private InputBuffer inputBuffer;

        [Header("Combat Data")]
        [SerializeField] private AttackDataSO[] lightAttacks;
        [SerializeField] private AttackDataSO heavyAttack;

        public bool IsCancelWindowOpen { get; private set; }
        public InputBuffer InputBuffer => inputBuffer;

        private IPlayerLocomotion _locomotionContext;

        private void Awake()
        {
            _locomotionContext = GetComponent<IPlayerLocomotion>();
            InitializeStateMachine();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerAttackLightMessage>(OnAttackLight);
            EventBus.Subscribe<PlayerAttackHeavyMessage>(OnAttackHeavy);
            EventBus.Subscribe<AnimationCancelWindowMessage>(OnAnimationCancelWindow);
            EventBus.Subscribe<AnimationFinishAttackMessage>(OnAnimationFinishAttack);
            EventBus.Subscribe<PlayerDashMessage>(OnDashMessage);
            EventBus.Subscribe<PlayerJumpMessage>(OnJumpMessage);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerAttackLightMessage>(OnAttackLight);
            EventBus.Unsubscribe<PlayerAttackHeavyMessage>(OnAttackHeavy);
            EventBus.Unsubscribe<AnimationCancelWindowMessage>(OnAnimationCancelWindow);
            EventBus.Unsubscribe<AnimationFinishAttackMessage>(OnAnimationFinishAttack);
            EventBus.Unsubscribe<PlayerDashMessage>(OnDashMessage);
            EventBus.Unsubscribe<PlayerJumpMessage>(OnJumpMessage);
        }

        private void Update()
        {
            var stateName = stateMachine.CurrentState?.StateName;
            if (stateName != "PlayerIdleState" && stateName != "PlayerWalkState") return;

            if (inputBuffer.HasCommand<PlayerAttackLightMessage>())
            {
                inputBuffer.ConsumeCommand<PlayerAttackLightMessage>();
                stateMachine.ChangeState(stateMachine.GetOrCreateState<PlayerAttackLight1State>());
                return;
            }

            if (inputBuffer.HasCommand<PlayerAttackHeavyMessage>())
            {
                inputBuffer.ConsumeCommand<PlayerAttackHeavyMessage>();
                stateMachine.ChangeState(stateMachine.GetOrCreateState<PlayerAttackHeavyState>());
            }
        }

        private void InitializeStateMachine()
        {
            stateMachine.RegisterState<PlayerAttackLight1State>(new StateFactory<PlayerAttackLight1State>
                (sm => new PlayerAttackLight1State(sm, this, _locomotionContext)));

            stateMachine.RegisterState<PlayerAttackLight2State>(new StateFactory<PlayerAttackLight2State>
                (sm => new PlayerAttackLight2State(sm, this, _locomotionContext)));

            stateMachine.RegisterState<PlayerAttackHeavyState>(new StateFactory<PlayerAttackHeavyState>
                (sm => new PlayerAttackHeavyState(sm, this, _locomotionContext)));
        }

        private void OnAttackLight(PlayerAttackLightMessage message)
        {
            if (lightAttacks == null || lightAttacks.Length == 0 || lightAttacks[0] == null) return;
            
            inputBuffer.BufferCommand<PlayerAttackLightMessage>(lightAttacks[0].BufferTime);
        }

        private void OnAttackHeavy(PlayerAttackHeavyMessage message)
        {
            if (heavyAttack == null) return;
            
            inputBuffer.BufferCommand<PlayerAttackHeavyMessage>(heavyAttack.BufferTime);
        }
        
        private void OnAnimationCancelWindow(AnimationCancelWindowMessage message)
        {
            IsCancelWindowOpen = message.IsOpen;
        }

        private void OnAnimationFinishAttack(AnimationFinishAttackMessage message)
        {
            IsCancelWindowOpen = false;
            inputBuffer.Clear();
            EventBus.Publish(new EndCombatMessage());
        }

        private void OnDashMessage(PlayerDashMessage message) => HandleInterrupt();
        private void OnJumpMessage(PlayerJumpMessage message) => HandleInterrupt();

        private void HandleInterrupt()
        {
            if (!IsCancelWindowOpen) return;

            var stateName = stateMachine.CurrentState?.StateName;
            if (stateName == "PlayerAttackLight1State" || stateName == "PlayerAttackLight2State")
            {
                IsCancelWindowOpen = false;
                inputBuffer.Clear();
                EventBus.Publish(new EndCombatMessage());
            }
        }

        public AttackDataSO GetLightAttackData(int comboIndex)
        {
            if (lightAttacks != null && comboIndex >= 0 && comboIndex < lightAttacks.Length)
                return lightAttacks[comboIndex];
                
            return null;
        }

        public AttackDataSO GetHeavyAttackData()
        {
            return heavyAttack;
        }
    }
}
