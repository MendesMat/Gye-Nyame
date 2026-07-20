using GyeNyame.Core.InputBuffer;
using GyeNyame.Combat.Data;

namespace GyeNyame.Combat.Contracts.Interfaces
{
    public interface IEntityCombatContext
    {
        bool IsCancelWindowOpen { get; }
        InputBuffer InputBuffer { get; }
        AttackDataSO CurrentAttackData { get; }
        void CloseCancelWindow();
    }
}
