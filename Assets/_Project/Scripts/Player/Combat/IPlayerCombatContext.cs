using GyeNyame.Core.InputBuffer;
using GyeNyame.Combat.Data;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Player.Combat
{
    public interface IPlayerCombatContext
    {
        bool IsCancelWindowOpen { get; }
        InputBuffer InputBuffer { get; }
        AttackDataSO CurrentAttackData { get; }
        void CloseCancelWindow();
    }
}
