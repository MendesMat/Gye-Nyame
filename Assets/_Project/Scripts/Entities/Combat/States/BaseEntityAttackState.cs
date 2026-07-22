using UnityEngine;
using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Combat.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Entities.Combat.States
{
    public abstract class BaseEntityAttackState : BaseState
    {
        protected readonly IEntityCombatContext combatContext;
        protected readonly IEntityLocomotion locomotionContext;

        public virtual bool AllowInterrupt => true;
        protected float stateEnterTime;

        protected BaseEntityAttackState(IStateMachine stateMachine, IEntityCombatContext combatContext, IEntityLocomotion locomotionContext) 
            : base(stateMachine)
        {
            this.combatContext = combatContext;
            this.locomotionContext = locomotionContext;
        }

        public override void Enter()
        {
            stateEnterTime = UnityEngine.Time.time;
            locomotionContext?.SetFacingDirectionLock(true);
            combatContext.ResetCombatState();
        }

        public override void FixedUpdate() => locomotionContext?.UpdateMovement(0f, false);

        public override void Exit()
        {
            base.Exit();
            locomotionContext?.SetFacingDirectionLock(false);
        }

        public virtual void OnAnimationFinish()
        {
            if (UnityEngine.Time.time - stateEnterTime < 0.1f) return;
            
            combatContext.ResetCombatState();
            var targetGo = ((MonoBehaviour)combatContext).gameObject;
            Core.Events.EventBus.Publish(new EndCombatMessage(targetGo));
        }
    }
}
