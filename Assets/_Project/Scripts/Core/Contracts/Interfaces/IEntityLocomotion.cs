using UnityEngine;

namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IEntityLocomotion
    {
        bool HasMoveInput { get; }
        float FacingDirectionX { get; }
        bool IsGrounded { get; }
        float VerticalVelocity { get; }
        void UpdateMovement(float speedMultiplier, bool lockDepth);
        void SetFacingDirectionLock(bool isLocked);
        void DisablePhysics();
        void ApplyExternalForce(Vector3 direction, float force, float verticalVelocity);
    }
}
