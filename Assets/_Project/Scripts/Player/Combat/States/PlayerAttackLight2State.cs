using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Player.Combat.States
{
    public class PlayerAttackLight2State : BaseState
    {
        private readonly IPlayerCombatContext _combatContext;
        private readonly Core.Contracts.Interfaces.IPlayerLocomotion _locomotionContext;

        public PlayerAttackLight2State(IStateMachine stateMachine, IPlayerCombatContext combatContext, IPlayerLocomotion locomotionContext) 
        : base(stateMachine)
        {
            _combatContext = combatContext;
            _locomotionContext = locomotionContext;
        }

        public override void Enter()
        {
            _combatContext.InputBuffer.ConsumeCommand<PlayerAttackLightMessage>();
        }

        public override void FixedUpdate()
        {
            _locomotionContext?.UpdateMovement(0f, false);
        }

        public override void Update()
        {
            if (!_combatContext.IsCancelWindowOpen) return;

            if (_combatContext.InputBuffer.HasCommand<PlayerAttackHeavyMessage>())
            {
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerAttackHeavyState>());
            }
        }
    }
}
