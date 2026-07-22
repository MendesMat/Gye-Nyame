using UnityEngine;

namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IEntityLocomotion
    {
        bool HasMoveInput { get; }
        float FacingDirectionX { get; }
        bool IsGrounded { get; }
        float VerticalVelocity { get; }
        Vector2 CurrentMoveInput { get; }
        Vector2 FacingDirection { get; }
        float AirSpeedMultiplier { get; }
        bool LockDepthDuringJump { get; }
        float DashSpeedMultiplier { get; }
        float DashDuration { get; }

        void UpdateMovement(float speedMultiplier, bool lockDepth);
        void SetFacingDirectionLock(bool isLocked);
        void ForceFacingDirectionX(float dirX);
        void DisablePhysics();
        void ApplyExternalForce(Vector3 direction, float force, float verticalVelocity);
        bool ConsumeJumpRequest();
        bool ConsumeDashRequest();
        void ExecuteJump();
        void ExecuteDash();
        void UpdateDirectionalMovement(Vector2 direction, float speedMultiplier);
    }
}
