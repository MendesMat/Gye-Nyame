namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IEnemyCombat
    {
        bool IsAttacking { get; }
        bool IsInCooldown { get; }
        void TryAttack();
    }
}
