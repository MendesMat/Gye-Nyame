using GyeNyame.Core.Contracts.Data;

namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IHurtbox
    {
        void ReceiveHit(DamageData damageData);
    }
}
