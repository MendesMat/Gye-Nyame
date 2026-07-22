using UnityEngine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.StateMachine;
using GyeNyame.Entities.Combat.States;
using GyeNyame.Combat.Data;
using GyeNyame.Combat.Contracts.Interfaces;
using GyeNyame.Combat.Components;

namespace GyeNyame.Enemy.Combat
{
    [RequireComponent(typeof(StateMachine))]
    public class EnemyCombat : MonoBehaviour, IEnemyCombat, IEntityCombatContext
    {
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private AttackDataSO attackData;
        [SerializeField] private HitboxComponent hitboxComponent;

        private IEntityLocomotion _locomotionContext;
        private float _cooldownEndTime;

        public bool IsCancelWindowOpen { get; private set; }
        public bool IsAttacking => stateMachine.CurrentState is BaseEntityAttackState;
        public bool IsInCooldown => Time.time < _cooldownEndTime;
        public AttackDataSO CurrentAttackData => attackData;

        private void Awake()
        {
            _locomotionContext = GetComponent<IEntityLocomotion>();
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            stateMachine.RegisterState<GenericEntityAttackState>(new StateFactory<GenericEntityAttackState>
                (sm => new GenericEntityAttackState(sm, this, _locomotionContext)));
        }

        public void TryAttack()
        {
            if (stateMachine.CurrentState is BaseEntityAttackState) return;

            stateMachine.ChangeState(stateMachine.GetOrCreateState<GenericEntityAttackState>());
        }
        
        public void ResetCombatState() => IsCancelWindowOpen = false;

        public void FinishAttack()
        {
            if (stateMachine.CurrentState is BaseEntityAttackState attackState)
            {
                StartCooldown();
                attackState.OnAnimationFinish();
            }
        }

        private void StartCooldown()
        {
            _cooldownEndTime = Time.time + attackData.CooldownTime;
        }

        public void OpenCancelWindow() => IsCancelWindowOpen = true;
        public void CloseCancelWindow() => IsCancelWindowOpen = false;

        public void OpenHitbox()
        {
            if (hitboxComponent == null) return;
            hitboxComponent.EnableHitbox();
        }

        public void CloseHitbox()
        {
            if (hitboxComponent == null) return;
            hitboxComponent.DisableHitbox();
        }
    }
}
