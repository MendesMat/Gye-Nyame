using UnityEngine;
using GyeNyame.Core.StateMachine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Player.Combat.States
{
    public class PlayerDeadState : Agents.Combat.States.EntityDeadState
    {
        private bool _slowMotionFinished;
        private float _slowMotionTimer;
        private const float SlowMotionDuration = 2f;
        private const float SlowMotionScale = 0.2f;

        public PlayerDeadState(IStateMachine stateMachine, IEntityLocomotion locomotionContext, IEntityHealth healthContext, IEntityVisuals visualsContext) 
            : base(stateMachine, locomotionContext, healthContext, visualsContext)
        { }

        public override void Enter()
        {
            base.Enter();
            _slowMotionFinished = false;
            _slowMotionTimer = SlowMotionDuration;
            
            EventBus.Publish(new SlowMotionRequestMessage(SlowMotionScale, SlowMotionDuration));
        }

        public override void Update()
        {
            if (!_slowMotionFinished)
            {
                _slowMotionTimer -= Time.unscaledDeltaTime;
                if (_slowMotionTimer <= 0f) _slowMotionFinished = true;
            }

            base.Update();
        }

        public override void FinishDeath()
        {
            EventBus.Publish(new GameOverMessage());
            base.FinishDeath();
        }
    }
}
