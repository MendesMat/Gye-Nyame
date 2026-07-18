using UnityEngine;

namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IEntityLocomotion
    {
        float FacingDirectionX { get; }
        void UpdateMovement(float speedMultiplier, bool lockDepth);
        void SetFacingDirectionLock(bool isLocked);
        void ApplyExternalForce(Vector3 direction, float force, float verticalVelocity);
        void ReturnToIdle();
    }
}
