using UnityEngine;

namespace GyeNyame.Core.Contracts.Interfaces
{
    public interface IKinematicPhysics
    {
        bool CheckGround(Vector3 currentPosition, float fallDistance, out float allowedFallDistance);
        Vector3 CalculateAllowedMovement(Vector3 currentPosition, Vector3 intendedMovement);
    }
}
