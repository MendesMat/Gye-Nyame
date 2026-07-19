using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Player.Combat.States
{
    public class GenericPlayerAttackState : BasePlayerAttackState
    {
        public override EntityStateCategory StateCategory => combatContext.CurrentAttackData.AnimationCategory;

        public GenericPlayerAttackState(IStateMachine stateMachine, IPlayerCombatContext combatContext, IEntityLocomotion locomotionContext) 
            : base(stateMachine, combatContext, locomotionContext)
        {
        }

        public override void Enter()
        {
            base.Enter();
            combatContext.InputBuffer.ConsumeCommand<PlayerAttackLightMessage>();
            combatContext.InputBuffer.ConsumeCommand<PlayerAttackHeavyMessage>();
        }

        public override void Update() { }
    }
}
