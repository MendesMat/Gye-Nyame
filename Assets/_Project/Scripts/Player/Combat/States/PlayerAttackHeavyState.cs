using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Player.Combat.States
{
    public class PlayerAttackHeavyState : BaseState
    {
        private readonly IPlayerCombatContext _combatContext;
        private readonly Core.Contracts.Interfaces.IEntityLocomotion _locomotionContext;

        public PlayerAttackHeavyState(IStateMachine stateMachine, IPlayerCombatContext combatContext, IEntityLocomotion locomotionContext) 
        : base(stateMachine)
        {
            _combatContext = combatContext;
            _locomotionContext = locomotionContext;
        }

        public override void Enter()
        {
            _locomotionContext?.SetFacingDirectionLock(true);
            _combatContext.InputBuffer.ConsumeCommand<PlayerAttackHeavyMessage>();
        }

        public override void FixedUpdate()
        {
            _locomotionContext?.UpdateMovement(0f, false);
        }

        public override void Exit()
        {
            base.Exit();
            _locomotionContext?.SetFacingDirectionLock(false);
        }
    }
}
