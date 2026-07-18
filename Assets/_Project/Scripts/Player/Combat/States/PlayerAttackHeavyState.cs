using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Player.Combat.States
{
    public class PlayerAttackHeavyState : BasePlayerAttackState
    {
        public override bool AllowInterrupt => false;
        public override EntityStateCategory StateCategory => EntityStateCategory.AttackHeavy;

        public PlayerAttackHeavyState(IStateMachine stateMachine, IPlayerCombatContext combatContext, IEntityLocomotion locomotionContext) 
            : base(stateMachine, combatContext, locomotionContext)
        {
        }

        public override void Enter()
        {
            base.Enter();
            combatContext.InputBuffer.ConsumeCommand<PlayerAttackHeavyMessage>();
        }
    }
}
