using GyeNyame.Core.StateMachine;
using GyeNyame.Core.InputBuffer;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Entities.Combat.States;
using GyeNyame.Entities.Movement.States;
namespace GyeNyame.Player.Combat.States
{
    public class PlayerHurtState : EntityHurtState
    {
        private readonly InputBuffer _inputBuffer;

        public PlayerHurtState(IStateMachine stateMachine, InputBuffer inputBuffer, IEntityLocomotion locomotionContext, IEntityHealth healthContext) 
            : base(stateMachine, locomotionContext, healthContext)
        {
            _inputBuffer = inputBuffer;
        }

        public override void Enter()
        {
            base.Enter();
            _inputBuffer?.Clear();
        }

        protected override void TransitionToIdle()
        {
            StateMachine.ChangeState(StateMachine.GetOrCreateState<EntityIdleState>());
        }

        protected override void TransitionToDeath()
        {
            StateMachine.ChangeState(StateMachine.GetOrCreateState<PlayerDeadState>());
        }
    }
}
