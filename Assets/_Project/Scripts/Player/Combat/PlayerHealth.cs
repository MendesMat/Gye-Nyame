using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.StateMachine;
using GyeNyame.Player.Combat.States;
using GyeNyame.Core.Events;
using GyeNyame.Combat.Components;

namespace GyeNyame.Player.Combat
{
    public class PlayerHealth : EntityHealth
    {
        [SerializeField] private StateMachine _stateMachine;

        protected override void OnDamageReceived(DamageData data)
        {
            base.OnDamageReceived(data);

            var hurtState = _stateMachine.GetOrCreateState<PlayerHurtState>();
            hurtState.InitializeHurt(data, transform.root.position);
            
            _stateMachine.ChangeState(hurtState);
        }

        protected override void Die()
        {
            base.Die();
            Debug.Log("Game Over! Player morreu.");
        }
    }
}
