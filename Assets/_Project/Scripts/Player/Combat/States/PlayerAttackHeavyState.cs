using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Player.Combat.States
{
    public class PlayerAttackHeavyState : BaseState
    {
        private readonly IPlayerCombatContext _combatContext;
        private readonly Core.Contracts.Interfaces.IPlayerLocomotion _locomotionContext;

        public PlayerAttackHeavyState(IStateMachine stateMachine, IPlayerCombatContext combatContext, IPlayerLocomotion locomotionContext) 
        : base(stateMachine)
        {
            _combatContext = combatContext;
            _locomotionContext = locomotionContext;
        }

        public override void Enter()
        {
            _combatContext.InputBuffer.ConsumeCommand<PlayerAttackHeavyMessage>();
        }

        public override void FixedUpdate()
        {
            _locomotionContext?.UpdateMovement(0f, false);
        }
    }
}
