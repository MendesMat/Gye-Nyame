using GyeNyame.Core.StateMachine;
using GyeNyame.Core.InputBuffer;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Agents.Combat.States;

namespace GyeNyame.Player.Combat.States
{
    public class PlayerHurtState : EntityHurtState
    {
        private readonly InputBuffer _inputBuffer;

        public PlayerHurtState(IStateMachine stateMachine, InputBuffer inputBuffer, IEntityLocomotion locomotionContext) 
            : base(stateMachine, locomotionContext)
        {
            _inputBuffer = inputBuffer;
        }

        public override void Enter()
        {
            base.Enter();
            _inputBuffer?.Clear();
        }
    }
}
