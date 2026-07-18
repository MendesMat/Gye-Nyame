using UnityEngine;

namespace GyeNyame.Player.Movement
{
    public interface IPlayerMovementContext : Core.Contracts.Interfaces.IEntityLocomotion
    {
        Vector2 CurrentMoveInput { get; }
        Vector2 FacingDirection { get; }

        float AirSpeedMultiplier { get; }
        bool LockDepthDuringJump { get; }
        float DashSpeedMultiplier { get; }
        float DashDuration { get; }

        bool ConsumeJumpRequest();
        bool ConsumeDashRequest();

        void ExecuteJump();
        void ExecuteDash();
        void UpdateDirectionalMovement(Vector2 direction, float speedMultiplier);
    }
}
