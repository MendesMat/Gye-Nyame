using System.Collections.Generic;
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
        [SerializeField] private CombatDirector combatDirector;

        private readonly Dictionary<AttackDataSO, IHitbox> _hitboxMap = new();

        private IEntityLocomotion _locomotionContext;
        private IAttackDirector _attackDirector;
        private float _cooldownEndTime;

        public bool IsCancelWindowOpen { get; private set; }
        public bool IsAttacking => stateMachine.CurrentState is BaseEntityAttackState;
        public bool IsInCooldown => Time.time < _cooldownEndTime;
        public AttackDataSO CurrentAttackData => attackData;

        private void Awake()
        {
            _locomotionContext = GetComponent<IEntityLocomotion>();
            _attackDirector = combatDirector;

            if (_attackDirector == null)
            {
                _attackDirector = FindAnyObjectByType<CombatDirector>();
            }
            
            InitializeHitboxes();
            InitializeStateMachine();
        }

        private void InitializeHitboxes()
        {
            HitboxComponent[] hitboxes = GetComponentsInChildren<HitboxComponent>(true);
            foreach (HitboxComponent hitbox in hitboxes)
            {
                if (hitbox.BoundAttackData == null) continue;
                _hitboxMap[hitbox.BoundAttackData] = hitbox;
            }
        }

        private void InitializeStateMachine()
        {
            stateMachine.RegisterState<GenericEntityAttackState>(new StateFactory<GenericEntityAttackState>
                (sm => new GenericEntityAttackState(sm, this, _locomotionContext)));
        }

        private void OnEnable()
        {
            _attackDirector?.RegisterAttacker(gameObject);
        }

        private void OnDisable()
        {
            _attackDirector?.UnregisterAttacker(gameObject);
        }

        public void TryAttack()
        {
            if (IsInCooldown) return;
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
            if (!_hitboxMap.TryGetValue(attackData, out IHitbox hitbox)) return;
            hitbox.EnableHitbox();
        }

        public void CloseHitbox()
        {
            if (!_hitboxMap.TryGetValue(attackData, out IHitbox hitbox)) return;
            hitbox.DisableHitbox();
        }
    }
}
