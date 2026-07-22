using GyeNyame.Combat.Data;

namespace GyeNyame.Combat.Contracts.Interfaces
{
    public interface IEntityCombatContext
    {
        bool IsCancelWindowOpen { get; }
        AttackDataSO CurrentAttackData { get; }
        void ResetCombatState();
    }
}
