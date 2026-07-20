using UnityEngine;
using GyeNyame.Entities.Movement;

namespace GyeNyame.Enemy.Movement
{
    public class EnemyMovement : BaseEntityMovement
    {
        public void MoveTowards(Vector2 targetDirection)
        {
            currentMoveInput = targetDirection;
            
            if (isFacingDirectionLocked) return;
            UpdateFacingDirection();
        }

        public void StopMoving() => currentMoveInput = Vector2.zero;
    }
}
