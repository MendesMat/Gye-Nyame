using GyeNyame.Core.Contracts.Data;

namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(DamageData damageData);
    }
}
