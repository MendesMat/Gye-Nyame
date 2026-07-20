namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IEntityVisuals
    {
        float FadeDuration { get; }
        void SetAlpha(float alpha);
        void ResetVisuals();
    }
}
