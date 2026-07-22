using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Combat.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Entities.Combat.States
{
    public class GenericEntityAttackState : BaseEntityAttackState
    {
        public override EntityStateCategory StateCategory => combatContext.CurrentAttackData.AnimationCategory;

        public GenericEntityAttackState(IStateMachine stateMachine, IEntityCombatContext combatContext, IEntityLocomotion locomotionContext) 
            : base(stateMachine, combatContext, locomotionContext)
        { }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update() { }
    }
}
