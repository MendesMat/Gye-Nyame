namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IEntityHealth
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsDead { get; }
        void SetDeadLayer(bool isDeadLayer);
    }
}
