namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IPlayerLocomotion
    {
        float FacingDirectionX { get; }
        void UpdateMovement(float speedMultiplier, bool lockDepth);
        void SetFacingDirectionLock(bool isLocked);
    }
}
