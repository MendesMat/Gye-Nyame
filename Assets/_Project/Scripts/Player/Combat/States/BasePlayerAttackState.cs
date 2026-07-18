using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Player.Combat.States
{
    public abstract class BasePlayerAttackState : BaseState
    {
        protected readonly IPlayerCombatContext combatContext;
        protected readonly IEntityLocomotion locomotionContext;

        public virtual bool AllowInterrupt => true;
        protected float stateEnterTime;

        protected BasePlayerAttackState(IStateMachine stateMachine, IPlayerCombatContext combatContext, IEntityLocomotion locomotionContext) 
            : base(stateMachine)
        {
            this.combatContext = combatContext;
            this.locomotionContext = locomotionContext;
        }

        public override void Enter()
        {
            stateEnterTime = UnityEngine.Time.time;
            locomotionContext?.SetFacingDirectionLock(true);
            combatContext.CloseCancelWindow();
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
            
            combatContext.CloseCancelWindow();
            combatContext.InputBuffer.Clear();
            Core.Events.EventBus.Publish(new EndCombatMessage());
        }
    }
}
