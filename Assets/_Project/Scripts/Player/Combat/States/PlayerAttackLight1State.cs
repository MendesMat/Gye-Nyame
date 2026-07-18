using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Player.Combat.States
{
    public class PlayerAttackLight1State : BasePlayerAttackState
    {
        public override EntityStateCategory StateCategory => EntityStateCategory.AttackLight1;

        public PlayerAttackLight1State(IStateMachine stateMachine, IPlayerCombatContext combatContext, IEntityLocomotion locomotionContext) 
            : base(stateMachine, combatContext, locomotionContext)
        {
        }

        public override void Enter()
        {
            base.Enter();
            combatContext.InputBuffer.ConsumeCommand<PlayerAttackLightMessage>();
        }

        public override void Update()
        {
            if (!combatContext.IsCancelWindowOpen) return;

            if (combatContext.InputBuffer.HasCommand<PlayerAttackLightMessage>())
            {
                combatContext.InputBuffer.ConsumeCommand<PlayerAttackLightMessage>();
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerAttackLight2State>());
                return;
            }
            
            if (combatContext.InputBuffer.HasCommand<PlayerAttackHeavyMessage>())
            {
                combatContext.InputBuffer.ConsumeCommand<PlayerAttackHeavyMessage>();
                StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerAttackHeavyState>());
            }
        }
    }
}
