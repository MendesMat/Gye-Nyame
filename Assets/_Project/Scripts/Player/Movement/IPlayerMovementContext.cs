namespace GyeNyame.Player.Movement
{
    public interface IPlayerMovementContext
    {
        bool HasMoveInput { get; }
        bool IsGrounded { get; }
        
        float AirSpeedMultiplier { get; }
        bool LockDepthDuringJump { get; }

        bool ConsumeJumpRequest();
        void ExecuteJump();
        void UpdateMovement(float speedMultiplier, bool lockDepth);
    }
}
