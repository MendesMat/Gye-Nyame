using GyeNyame.Core.InputBuffer;

namespace GyeNyame.Player.Combat
{
    public interface IPlayerCombatContext
    {
        bool IsCancelWindowOpen { get; }
        InputBuffer InputBuffer { get; }
        
        AttackDataSO GetLightAttackData(int comboIndex);
        AttackDataSO GetHeavyAttackData();
    }
}
