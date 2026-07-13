using UnityEngine;

namespace GyeNyame.Player.Movement
{
    public interface IPlayerMovementContext
    {
        bool HasMoveInput { get; }
        bool IsGrounded { get; }
        Vector2 CurrentMoveInput { get; }
        Vector2 FacingDirection { get; }
        float FacingDirectionX { get; }

        float AirSpeedMultiplier { get; }
        bool LockDepthDuringJump { get; }
        float DashSpeedMultiplier { get; }
        float DashDuration { get; }

        bool ConsumeJumpRequest();
        bool ConsumeDashRequest();

        void ExecuteJump();
        void ExecuteDash();
        void UpdateMovement(float speedMultiplier, bool lockDepth);
        void UpdateDirectionalMovement(Vector2 direction, float speedMultiplier);
    }
}
